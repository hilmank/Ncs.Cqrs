using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.Reservations.DTOs;

public class UpdateReservationsDto
{
    [Required]
    [SwaggerSchema("ID of the updated menu item")]
    public int MenuItemsId { get; set; }

    [SwaggerSchema("Indicates if the updated reservation menu is spicy (true) or regular (false)")]
    public bool IsSpicy { get; set; } = false;

    [SwaggerSchema("List of guests included in the updated reservation")]
    public List<UpdateReservationGuestsDto> Guests { get; set; } = new List<UpdateReservationGuestsDto>();
}
