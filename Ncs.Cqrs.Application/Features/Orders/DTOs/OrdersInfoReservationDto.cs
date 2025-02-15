namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class OrdersInfoReservationDto
    {
        public int ReservationsId { get; set; }
        public int MenuItemsId { get; set; }
        public string Name { get; set; }
        public bool IsSpicy { get; set; }
        public string CreatedAt { get; set; }
        public List<OrdersInfoReservationGuestsDto> Guests { get; set; }
    }
}
