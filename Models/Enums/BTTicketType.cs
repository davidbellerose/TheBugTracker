using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models.Enums
{
    public enum BTTicketType
    {
        [Display(Name = "New Development")]
        NewDevelopment,
        [Display(Name = "Work Task")]
        WorkTask,
        Defect,
        [Display(Name = "Change Request")]
        ChangeRequest,
        Enhancement,
        [Display(Name = "General Task")]
        GeneralTask
    }
}
