using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TheBugTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser BTUser { get; set; }

        public MultiSelectList Roles { get; set; }

        public MultiSelectList Companies { get; set; }

        //public List<string> SelectedRoles { get; set; }

        public string SelectedRoles { get; set; }
    }
}
