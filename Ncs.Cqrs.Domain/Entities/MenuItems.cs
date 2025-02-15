using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("menu_items", Schema = "public")]
    public class MenuItems
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("calories")]
        public double Calories { get; set; }

        [Required]
        [Column("price")]
        public double Price { get; set; }

        [Required]
        [Column("image_url")]
        public string ImageUrl { get; set; }

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

        // Navigation properties
        [ForeignKey("vendor_id")]
        public virtual Vendors Vendor { get; set; }

        [ForeignKey("created_by")]
        public virtual Users CreatedByUser { get; set; }

        [ForeignKey("updated_by")]
        public virtual Users UpdatedByUser { get; set; }
    }
}
