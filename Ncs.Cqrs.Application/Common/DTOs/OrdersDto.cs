using Ncs.Cqrs.Domain.Entities;

namespace Ncs.Cqrs.Application.Common.DTOs
{
    public class OrdersDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ReservationGuestsId { get; set; }
        public int MenuItemsId { get; set; }
        public bool IsSpicy { get; set; }
        public int? ReservationsId { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderDate { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual UsersDto UserOrder { get; set; }
        public virtual ReservationGuestsDto ReservationGuests { get; set; }
        public virtual MenuItemsDto MenuItem { get; set; }
        public virtual ReservationsDto Reservation { get; set; }
        public virtual UsersDto CreatedByUser { get; set; }
        public virtual UsersDto UpdatedByUser { get; set; }
    }
}
