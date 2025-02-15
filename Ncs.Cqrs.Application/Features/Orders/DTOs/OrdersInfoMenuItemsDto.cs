namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class OrdersInfoMenuItemsDto
    {
        public int MenuItemsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Calories { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
