using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.DTOs
{
    /// <summary>
    /// Data transfer object for updating a menu schedule.
    /// </summary>
    public class UpdateMenuSchedulesDto
    {

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Available quantity must be greater than zero.")]
        [SwaggerSchema("The updated available quantity of meals.")]
        public int AvailableQuantity { get; set; }
    }
}
