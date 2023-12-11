using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TheBugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }


        // Navigation
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        // relationship to Invite model
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();

    }
}
