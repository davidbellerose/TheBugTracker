using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace TheBugTracker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }



        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile AvatarFormFile { get; set; }

        [DisplayName("Avatar")]
        public string AvatarFileName { get; set; }

        public byte[] AvatarFileData { get; set; }


        [DisplayName("File Extension")]
        public string AvatarFileContentType { get; set; }


        // Foreign Keys / Navigation
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}
