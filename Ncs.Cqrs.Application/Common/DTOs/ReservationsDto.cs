using Ncs.Cqrs.Application.Common.DTOs;

public class ReservationsDto
{
    public int Id { get; set; }
    public int ReservedBy { get; set; } // User ID
    public string ReservedDate { get; set; }
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; }
    public int StatusId { get; set; }
    public string CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation properties
    public virtual UsersDto ReservedByUser { get; set; }
    public virtual MenuItemsDto MenuItem { get; set; }
    public virtual ReservationsStatusDto Status { get; set; }
    public virtual UsersDto CreatedByUser { get; set; }
    public virtual UsersDto UpdatedByUser { get; set; }

    // List of guests in the reservation
    public List<ReservationGuestsDto> Guests { get; set; } = new List<ReservationGuestsDto>();
}
