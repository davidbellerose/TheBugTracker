using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Extensions;
using Microsoft.AspNetCore.Identity;
using TheBugTracker.Services.Interfaces;
using TheBugTracker.Models.Enums;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _roleService;

        public NotificationsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService roleService)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            // need userId, only show notifications where sender or recipient is user
            // admin should see all
            BTUser btUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole(nameof(Roles.Administrator)))
            {
                var applicationDbContext = _context.Notifications.Include(n => n.Recipient).Include(n => n.Sender).Include(n => n.Ticket);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.Notifications.Include(n => n.Recipient).Include(n => n.Sender).Include(n => n.Ticket).Where(u => u.RecipientId == btUser.Id || u.SenderId == btUser.Id);

                return View(await applicationDbContext.ToListAsync());
            }
                
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {

            int companyId = User.Identity.GetCompanyId().Value;

            ViewData["RecipientId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName");
            ViewData["SenderId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName");
            ViewData["TicketId"] = new SelectList(_context.Tickets.Where(u => u.Project.CompanyId == companyId), "Id", "Title");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Message,Created,Viewed,TicketId,RecipientId,SenderId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            int companyId = User.Identity.GetCompanyId().Value;

            ViewData["RecipientId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "Id", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets.Where(u => u.Project.CompanyId == companyId), "Id", "Id", notification.TicketId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            ViewData["RecipientId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "FullName", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets.Where(u => u.Project.CompanyId == companyId), "Id", "Id", notification.TicketId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Message,Created,Viewed,TicketId,RecipientId,SenderId")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            int companyId = User.Identity.GetCompanyId().Value;

            ViewData["RecipientId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "Id", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users.Where(u => u.CompanyId == companyId), "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets.Where(u => u.Project.CompanyId == companyId), "Id", "Id", notification.TicketId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
