using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("users", Schema = "public")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Required]
        [Column("firstname")]
        public string Firstname { get; set; }

        [Column("middlename")]
        public string Middlename { get; set; }

        [Column("lastname")]
        public string Lastname { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column("address")]
        public string Address { get; set; }

        [Column("employee_number")]
        public string EmployeeNumber { get; set; }

        [Column("company_id")]
        public int? CompanyId { get; set; }

        [Column("personalid_type_id")]
        public int? PersonalTypeId { get; set; }

        [Column("personal_id_number")]
        public string PersonalIdNumber { get; set; }

        [Column("guest_company_name")]
        public string GuestCompanyName { get; set; }

        [Column("rfid_tag")]
        public string RfidTag { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }
        [NotMapped]
        public string Fullname => string.Join(" ", new[] { Firstname, Middlename, Lastname }.Where(s => !string.IsNullOrWhiteSpace(s)));
        // Navigation properties
        [ForeignKey("company_id")]
        public virtual Companies Company { get; set; }

        [ForeignKey("personalid_type_id")]
        public virtual PersonalIdType PersonalIdType { get; set; }

        [ForeignKey("created_by")]
        public virtual Users CreatedByUser { get; set; }

        [ForeignKey("updated_by")]
        public virtual Users UpdatedByUser { get; set; }
        // Many-to-Many Relationship: Users <-> Roles (via UsersRoles)
        public virtual ICollection<Roles> Roles { get; set; } = [];

    }
}