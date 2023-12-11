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
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            // new ticket has been added
            if (oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    UserId = userId,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    Description = "New Ticket Created"
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                // Check ticket title
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Title: {newTicket.Title}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                // check ticket description
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Description: {newTicket.Description}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                // check for ticket priority
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "TicketPriority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Priority: {newTicket.TicketPriority}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                // check for ticket status
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "TicketStatus",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Status: {newTicket.TicketStatus}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                // check for ticket type
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "TicketType",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Type: {newTicket.TicketType}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                // check ticket developer
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        UserId = userId,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        Description = $"New Ticket Developer: {newTicket.DeveloperUser.FullName}"
                    };

                    try
                    {
                        await _context.TicketHistories.AddAsync(history);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FindAsync(ticketId);
                string description = model.ToLower().Replace("ticket", "");
                description = $"New {description} added to ticket: {ticket.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    Property = model,
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = description
                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context.Companies
                                        .Include(c => c.Projects)
                                            .ThenInclude(p => p.Tickets)
                                                .ThenInclude(t => t.History)
                                                    .ThenInclude(h => h.User)
                                        .FirstOrDefaultAsync(c => c.Id == companyId)).Projects.ToList();

                List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();
                List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();
                return ticketHistories;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.History)
                                            .ThenInclude(h => h.User)
                                    .FirstOrDefaultAsync(p => p.Id == projectId);

                List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t => t.History).ToList();
                return ticketHistory;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
