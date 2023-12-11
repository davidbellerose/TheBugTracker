using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class BTInviteService : IBTInviteService
    {

        private readonly ApplicationDbContext _context;

        public BTInviteService(ApplicationDbContext context)
        {
            _context = context;
        }



        // this is when the user clicks the invite link in the sent email
        // the guid identifies the invite of the user and company
        // basically this method expires the invite after it is accepted.
        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
            //if this invite exists, it means the invitee has registered an account
            if (invite == null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false; //invite is accepted and therefore expired
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();

                return true;
            }

            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.Invites.AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //checks to see if any invite exists for this company
        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(i => i.Companyid == companyId)
                            .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }

        
        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.Companyid == companyId)
                                            .Include(i => i.Company)
                                            .Include(i => i.Project)
                                            .Include(i => i.Invitor)
                                            .FirstOrDefaultAsync(i => i.Id == inviteId);

                return invite;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.Companyid == companyId)
                                            .Include(i => i.Company)
                                            .Include(i => i.Project)
                                            .Include(i => i.Invitor)
                                    .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return invite;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // invite is valid for 7 days, this checks if the invite date is within this timeframe
        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if (token ==  null)
            {
                return false;
            }

            bool result = false;

            Invite invite = await _context.Invites.FirstOrDefaultAsync(i=>i.CompanyToken == token);

            if (invite != null) 
            {
                DateTime inviteDate = invite.InviteDate.DateTime;

                bool validDate = (DateTime.Now -  inviteDate).TotalDays <= 7;

                if (validDate)
                {
                    result = invite.IsValid;
                }
            }
            return result;
        }
    }
}
