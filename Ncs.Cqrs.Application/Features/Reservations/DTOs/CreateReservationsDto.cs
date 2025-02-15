using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.Reservations.DTOs;

public class CreateReservationsDto
{
    [Required]
    [SwaggerSchema("ID of the user making the reservation")]
    public int ReservedBy { get; set; } // User ID

    [Required]
    [SwaggerSchema("Date of the reservation in 'yyyy-MM-dd' format")]
    public string ReservedDate { get; set; } // Format: "yyyy-MM-dd"

    [Required]
    [SwaggerSchema("ID of the menu item to be reserved")]
    public int MenuItemsId { get; set; }

    [SwaggerSchema("Indicates if the reserved menu is spicy (true) or regular (false)")]
    public bool IsSpicy { get; set; } = false; // Default: Regular (false), Spicy (true)

    [SwaggerSchema("List of guests included in the reservation")]
    public List<CreateReservationGuestsDto> Guests { get; set; } = new List<CreateReservationGuestsDto>();
}

