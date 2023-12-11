using System.ComponentModel;
using System;
using System.Net.Sockets;

namespace TheBugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }


        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }






        // Foreign Key / Navigation
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }


        [DisplayName("Team Member")]
        public string UserId { get; set; }

        public virtual BTUser User { get; set; }
    }
}
