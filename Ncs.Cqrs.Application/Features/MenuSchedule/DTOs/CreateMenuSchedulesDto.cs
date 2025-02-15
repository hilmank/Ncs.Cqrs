using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.DTOs
{
    /// <summary>
    /// Data transfer object for creating a menu schedule.
    /// </summary>
    public class CreateMenuSchedulesDto
    {
        [Required]
        [SwaggerSchema("The scheduled date for the menu (YYYY-MM-DD).")]
        public string ScheduleDate { get; set; }

        [Required]
        [SwaggerSchema("The menu item first ID that is scheduled.")]
        public int MenuItemsId1st { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Available quantity first menu must be greater than zero.")]
        [SwaggerSchema("The available quantity of first menu for this schedule.")]
        public int AvailableQuantity1st { get; set; }

        [Required]
        [SwaggerSchema("The menu item second ID that is scheduled.")]
        public int MenuItemsId2nd { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Available quantity second menu must be greater than zero.")]
        [SwaggerSchema("The available quantity of second menu for this schedule.")]
        public int AvailableQuantity2nd { get; set; }

        [SwaggerSchema("Indicates if the schedule should be applied for the entire week.")]
        public bool IsWeekly { get; set; }

        [SwaggerSchema("Indicates if the schedule should be applied for the entire month.")]
        public bool IsMonthly { get; set; }
    }
}
