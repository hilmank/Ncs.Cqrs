namespace Ncs.Cqrs.Application.Common.DTOs;

public class MenuItemsDto
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Calories { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public virtual VendorsDto Vendor { get; set; }
    public virtual UsersDto CreatedByUser { get; set; }
    public virtual UsersDto UpdatedByUser { get; set; }
}
