using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }


        [DisplayName("Date Sent")]
        [DataType(DataType.Date)]
        public DateTimeOffset InviteDate { get; set; }


        [DisplayName("Join Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }


        public bool IsValid { get; set; }

        // Foreign Keys / Navigation

        [DisplayName("Company")]
        public int? Companyid { get; set; }
        public virtual Company Company { get; set; }


        [DisplayName("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }



        [DisplayName("Inviter")]
        public string InvitorId { get; set; }
        public virtual BTUser Invitor { get; set; }


        [DisplayName("Invitee")]
        public string InviteeId { get; set; }
        public virtual BTUser Invitee { get; set; }


        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string InviteeLastName { get; set; }


    }
}
