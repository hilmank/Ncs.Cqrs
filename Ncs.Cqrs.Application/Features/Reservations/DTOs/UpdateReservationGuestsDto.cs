using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.Reservations.DTOs;

public class UpdateReservationGuestsDto
{
    [Required]
    [SwaggerSchema("ID of the guest to update")]
    public int Id { get; set; }

    [Required]
    [SwaggerSchema("Updated full name of the guest")]
    public string Fullname { get; set; }

    [Required]
    [SwaggerSchema("Updated company name of the guest")]
    public string CompanyName { get; set; }

    [Required]
    [SwaggerSchema("Updated type ID of the personal identification document")]
    public int PersonalIdTypeId { get; set; }

    [Required]
    [SwaggerSchema("Updated personal identification number of the guest")]
    public string PersonalIdNumber { get; set; }

    [Required]
    [SwaggerSchema("Updated ID of the menu item selected by the guest")]
    public int MenuItemsId { get; set; }

    [SwaggerSchema("Indicates if the updated guest's reserved menu is spicy (true) or regular (false)")]
    public bool IsSpicy { get; set; } = false;
}
