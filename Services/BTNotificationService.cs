using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTRolesService _rolesService;

        public BTNotificationService(ApplicationDbContext context, IEmailSender emailSender, IBTRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }




        public async Task AddNotificationAsyc(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        // gets the notifications user received
        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                            .Include(n => n.Recipient)
                                            .Include(n => n.SenderId)
                                            .Include(n => n.Ticket)
                                                .ThenInclude(t => t.ProjectId)
                                            .Where(n => n.RecipientId == userId)
                                            .ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //get notifications sent by user
        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                            .Include(n => n.Recipient)
                                            .Include(n => n.SenderId)
                                            .Include(n => n.Ticket)
                                                .ThenInclude(t => t.ProjectId)
                                            .Where(n => n.SenderId == userId)
                                            .ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // sends the notification email and returns true if successful
        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);
            if (btUser != null)
            {
                string btUserEmail = btUser.Email;
                string message = notification.Message;

                //send email
                try
                {
                    await _emailSender.SendEmailAsync(btUserEmail, emailSubject, message);
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        // send notification to all in that role
        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync (notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        //send to all company users
        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
