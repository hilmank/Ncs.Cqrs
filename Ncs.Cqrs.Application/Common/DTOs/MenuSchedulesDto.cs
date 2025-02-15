
namespace Ncs.Cqrs.Application.Common.DTOs
{
    public class MenuSchedulesDto
    {
        public int Id { get; set; }
        public int MenuItemsId { get; set; }

        public string Scheduledate { get; set; }
        public int AvailableQuantity { get; set; }

        public string CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
        public virtual MenuItemsDto MenuItem { get; set; }

        public virtual UsersDto CreatedByUser { get; set; }

        public virtual UsersDto UpdatedByUser { get; set; }
    }
}
