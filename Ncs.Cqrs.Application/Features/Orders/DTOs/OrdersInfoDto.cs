namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class OrdersInfoDto
    {
        public OrdersInfoUserDto User { get; set; }
        public List<OrdersInfoMenuItemsDto> MenuItems { get; set; }
        public OrdersInfoReservationDto? Reservation { get; set; }
    }
}
