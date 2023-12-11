using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {

        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }



        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR adding new ticket: {ex.Message}");
                throw;
            }
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.AddAsync(ticketComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  archiving ticket: {ex.Message}");
                throw;
            }
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            try
            {
                if (ticket != null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  assigning ticket: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int CompanyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                    .Where(p => p.CompanyId == CompanyId)
                    .SelectMany(p => p.Tickets)
                    .Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.History)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .ToListAsync();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting all tickets by company: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int CompanyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                        .Where(p => p.CompanyId == CompanyId)
                        .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.History)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketTypeId)
                        .Include(t => t.Project)
                        .Where(t => t.TicketPriorityId == priorityId)
                        .ToListAsync();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting all tickets by priority: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int CompanyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                        .Where(p => p.CompanyId == CompanyId)
                        .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.History)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketTypeId)
                        .Include(t => t.Project)
                        .Where(t => t.TicketStatusId == statusId)
                        .ToListAsync();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting all tickets by status: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int CompanyId, string typeName)
        {

            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                        .Where(p => p.CompanyId == CompanyId)
                        .SelectMany(p => p.Tickets)
                        .Include(t => t.Attachments)
                        .Include(t => t.Comments)
                        .Include(t => t.DeveloperUser)
                        .Include(t => t.History)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketTypeId)
                        .Include(t => t.Project)
                        .Where(t => t.TicketTypeId == typeId)
                        .ToListAsync();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting all tickets by type: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int CompanyId)
        {

            try
            {
                List<Ticket> tickets = (await GetAllTicketsByCompanyAsync(CompanyId))
                    .Where(t => t.Archived == true)
                    .ToList();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting archived tickets: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int CompanyId, int projectId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(CompanyId, priorityName)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting tickets by priority: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int CompanyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, CompanyId)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting tickets by role: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int CompanyId, int projectId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetAllTicketsByStatusAsync(CompanyId, statusName)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting tickets by status: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int CompanyId, int projectId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetAllTicketsByTypeAsync(CompanyId, typeName)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting tickets by type: {ex.Message}");
                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets

                        .Include(t => t.DeveloperUser)
                        .Include(t => t.Project)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.User)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets

                        .Include(t => t.DeveloperUser)
                        .Include(t => t.OwnerUser)
                        .Include(t => t.Project)
                        .Include(t => t.TicketPriority)
                        .Include(t => t.TicketStatus)
                        .Include(t => t.TicketType)
                        .Include(t => t.Comments)
                        .Include(t => t.Attachments)
                        .Include(t => t.History)
                    .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting ticket: {ex.Message}");
                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int CompanyId)
        {
            BTUser developer = new();
            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(CompanyId)).FirstOrDefault(t => t.Id == ticketId);
                if (ticket?.DeveloperUserId != null)
                {
                    developer = ticket.DeveloperUser;
                }
                return developer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting developer: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int CompanyId)
        {
            List<Ticket> tickets = new List<Ticket>();

            try
            {
                if (role == Roles.Administrator.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(CompanyId);
                }
                else if (role == Roles.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(CompanyId))
                        .Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(CompanyId))
                        .Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, CompanyId);
                }

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting ticket by role: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int CompanyId)
        {
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket> tickets = new List<Ticket>();

            try
            {
                if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Administrator.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(CompanyId))
                                .SelectMany(p => p.Tickets).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Developer.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(CompanyId))
                                .SelectMany(p => p.Tickets)
                                .Where(t => t.DeveloperUserId == userId)
                                .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(CompanyId))
                                .SelectMany(p => p.Tickets)
                                .Where(t => t.OwnerUserId == userId)
                                .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId))
                                .SelectMany(t => t.Tickets)
                                .ToList();
                }
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting tickets by user: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => string.IsNullOrEmpty(t.DeveloperUserId)).ToList();

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  getting unassigned tickets: {ex.Message}");
                throw;
            }
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(t => t.Name == priorityName);
                return priority?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  looking up ticket by priority: {ex.Message}");
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(t => t.Name == statusName);
                return status?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  looking up ticket by status: {ex.Message}");
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(t => t.Name == typeName);
                return type?.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  looking up ticket by type: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*********   ERROR  updating ticket: {ex.Message}");
                throw;
            }
        }
    }
}
