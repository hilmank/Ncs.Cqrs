using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.Reservations.DTOs;

public class CreateReservationGuestsDto
{
    [Required]
    [SwaggerSchema("Full name of the guest")]
    public string Fullname { get; set; }

    [Required]
    [SwaggerSchema("Company name of the guest")]
    public string CompanyName { get; set; }

    [Required]
    [SwaggerSchema("Type ID of the personal identification document")]
    public int PersonalIdTypeId { get; set; }

    [Required]
    [SwaggerSchema("Personal identification number of the guest")]
    public string PersonalIdNumber { get; set; }

    [Required]
    [SwaggerSchema("ID of the menu item selected by the guest")]
    public int MenuItemsId { get; set; }

    [SwaggerSchema("Indicates if the guest's reserved menu is spicy (true) or regular (false)")]
    public bool IsSpicy { get; set; } = false;
}
