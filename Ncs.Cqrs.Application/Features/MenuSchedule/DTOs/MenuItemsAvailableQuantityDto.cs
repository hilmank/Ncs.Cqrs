using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.DTOs
{
    public class MenuItemsAvailableQuantityDto
    {
        [Required]
        [SwaggerSchema("The menu item ID that is scheduled.")]
        public int MenuItemsId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Available quantity must be greater than zero.")]
        [SwaggerSchema("The available quantity of meals for this schedule.")]
        public int AvailableQuantity { get; set; }
    }
}
