using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace TheBugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be betwen {2} and {1} characters.", MinimumLength = 2)]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        public byte[] FileData { get; set; }


        [DisplayName("File Extension")]
        public string FileContentType { get; set; }

        public bool Archived { get; set; }

        // Foreign Keys / Navigation

        [DisplayName("Company")]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int? ProjectPriorityId { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
