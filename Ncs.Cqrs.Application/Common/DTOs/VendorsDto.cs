namespace Ncs.Cqrs.Application.Common.DTOs
{
    public class VendorsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public virtual UsersDto CreatedByUser { get; set; }

        public virtual UsersDto UpdatedByUser { get; set; }
    }
}
