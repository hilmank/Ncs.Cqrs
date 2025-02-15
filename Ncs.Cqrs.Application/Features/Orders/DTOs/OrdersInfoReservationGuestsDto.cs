namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class OrdersInfoReservationGuestsDto
    {
        public int ReservationGuestsId { get; set; }
        public string Fullname { get; set; }
        public string CompanyName { get; set; }
        public int MenuItemsId { get; set; }
        public string Name { get; set; }
        public bool IsSpicy { get; set; }
    }
}
