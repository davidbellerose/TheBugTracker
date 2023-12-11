using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheBugTracker.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Priority Name")]
        public string Name { get; set; }
    }
}
