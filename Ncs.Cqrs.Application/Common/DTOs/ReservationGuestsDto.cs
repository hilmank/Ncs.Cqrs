namespace Ncs.Cqrs.Application.Common.DTOs;

public class ReservationGuestsDto
{
    public int Id { get; set; }
    public int ReservationsId { get; set; } // Linked to the reservation
    public string Fullname { get; set; }
    public string CompanyName { get; set; }
    public int PersonalIdTypeId { get; set; }
    public string PersonalIdNumber { get; set; }
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; }

    // Navigation properties
    public virtual ReservationsDto Reservation { get; set; }
    public virtual PersonalIdTypeDto PersonalType { get; set; }
    public virtual MenuItemsDto MenuItem { get; set; }
}
