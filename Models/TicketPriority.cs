using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }


        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Priority Name")]
        public string Name { get; set; }
    }
}
