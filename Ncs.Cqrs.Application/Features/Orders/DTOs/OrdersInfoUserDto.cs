namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class OrdersInfoUserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
    }
}
