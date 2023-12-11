using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Title")]
        public string Title { get; set; }


        [DisplayName("Message")]
        public string Message { get; set; }


        [DisplayName("Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }


        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }


        // Foreign Keys / Navigation

        [DisplayName("Ticket")]
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }


        // These will have a GUID for the user which is why they are a string
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }
        public virtual BTUser Recipient { get; set; }


        [DisplayName("Sender")]
        public string SenderId { get; set; }
        public virtual BTUser Sender { get; set; }


    }
}
