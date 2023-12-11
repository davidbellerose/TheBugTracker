using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTFileService _fileService;
        private readonly IBTTicketHistoryService _ticketHistoryService;

        public TicketsController(UserManager<BTUser> userManager, IBTProjectService projectService, IBTLookupService lookupService, IBTTicketService ticketService, IBTFileService fileService, IBTTicketHistoryService ticketHistoryService)
        {
            _userManager = userManager;
            _projectService = projectService;
            _lookupService = lookupService;
            _ticketService = ticketService;
            _fileService = fileService;
            _ticketHistoryService = ticketHistoryService;
        }


        public async Task<IActionResult> MyTickets()
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(btUser.Id, btUser.CompanyId);

            return View(tickets);
        }

        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            if (User.IsInRole(nameof(Roles.DemoUser)) || User.IsInRole(nameof(Roles.Submitter)))
            {
                return View(tickets.Where(t => t.Archived == false));
            }
            else
            {
                return View(tickets.Where(t => t.Archived == false));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                try
                {
                    ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                    ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                    ticketAttachment.FileContentType = ticketAttachment.FormFile.ContentType;

                    ticketAttachment.Created = DateTimeOffset.Now;
                    ticketAttachment.UserId = _userManager.GetUserId(User);

                    await _ticketService.AddTicketAttachmentAsync(ticketAttachment);

                    await _ticketHistoryService.AddHistoryAsync(ticketAttachment.Id, nameof(TicketAttachment), ticketAttachment.UserId);


                }
                catch (Exception)
                {

                    throw;
                }

                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);
            return View(tickets);
        }

        [Authorize(Roles = "Administrator,ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            string btUserId = _userManager.GetUserId(User);

            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(Roles.Administrator)))
            {
                return View(tickets);
            }
            else
            {
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(btUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }
                return View(pmTickets);
            }

        }


        [Authorize(Roles = "Administrator,ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int ticketId)
        {
            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(Roles.Developer)), "Id", "FullName");

            return View(model);
        }


        [Authorize(Roles = "Administrator,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (model.DeveloperId != null)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);

                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                }
                catch (Exception)
                {

                    throw;
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                //return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });

                return RedirectToAction("Details", "Projects", new { id = model.Ticket.ProjectId });
            }


            return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket.Id });

            //return RedirectToAction(nameof(AssignDeveloper), new { model.DeveloperId });
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create(int? projectId)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Administrator)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name", projectId);
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name", projectId);
            }


            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            ViewData["id"] = projectId;
            



            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                try
                {
                    ticket.Created = DateTimeOffset.Now;
                    ticket.OwnerUserId = btUser.Id;
                    ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New))).Value;

                    await _ticketService.AddNewTicketAsync(ticket);

                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _ticketHistoryService.AddHistoryAsync(null, newTicket, btUser.Id);
                }
                catch (Exception)
                {

                    throw;
                }

                return RedirectToAction(nameof(AllTickets));
            }


            if (User.IsInRole(nameof(Roles.Administrator)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }


            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");


            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);


            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);


            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

                try
                {

                    ticket.Updated = DateTimeOffset.Now;
                    await _ticketService.UpdateTicketAsync(ticket);

                    ticket.OwnerUserId = btUser.Id;
                    //ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New))).Value;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                return RedirectToAction(nameof(AllTickets));
            }
            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,Comment")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.Created = DateTime.Now;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    await _ticketHistoryService.AddHistoryAsync(ticketComment.Id, nameof(TicketComment), ticketComment.UserId);

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }



        // GET: Tickets/Archive/5
        [Authorize(Roles = "Administrator,ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Archive/5
        [Authorize(Roles = "Administrator,ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = true;
            ticket.Updated = DateTimeOffset.Now;
            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }


        // GET: Tickets/Restore/5
        [Authorize(Roles = "Administrator,ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Restore/5
        [Authorize(Roles = "Administrator,ProjectManager")]
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = false;
            ticket.Updated = DateTimeOffset.Now;
            await _ticketService.UpdateTicketAsync(ticket);

            return RedirectToAction(nameof(AllTickets));
        }

        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}
