using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }


        [DisplayName("Previous")]
        public string OldValue { get; set; }


        [DisplayName("Current")]
        public string NewValue { get; set; }


        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }



        [DisplayName("Description of Change")]
        public string Description { get; set; }



        // navigation
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }





        [DisplayName("Team Member")]
        public string UserId { get; set; }

        public virtual BTUser User { get; set; }

    }
}
