using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }


        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Title")]
        public string Title { get; set; }


        [DisplayName("Description")]
        public string Description { get; set; }


        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }


        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }


        [DisplayName("Archived")]
        public bool Archived { get; set; }


        [DisplayName("Archived by Project")]
        public bool ArchivedByProject { get; set; }


        // Foreign Key / Navigation
        [DisplayName("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }


        [DisplayName("Type")]
        public int TicketTypeId { get; set; }
        public virtual TicketType TicketType { get; set; }


        [DisplayName("Priority")]
        public int TicketPriorityId { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }


        [DisplayName("Status")]
        public int TicketStatusId { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }


        [DisplayName("Owner")]
        public string OwnerUserId { get; set; }
        public virtual BTUser OwnerUser { get; set; }


        [DisplayName("Developer")]
        public string? DeveloperUserId { get; set; }
        public virtual BTUser? DeveloperUser { get; set; }


        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();


    }
}
