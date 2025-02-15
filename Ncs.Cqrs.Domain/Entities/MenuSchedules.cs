using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("menu_schedules", Schema = "public")]
    public class MenuSchedules
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("menu_items_id")]
        public int MenuItemsId { get; set; }

        [Required]
        [Column("schedule_date")]
        public DateTime ScheduleDate { get; set; }

        [Required]
        [Column("available_quantity")]
        public int AvailableQuantity { get; set; }

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
        [ForeignKey("menu_items_id")]
        public virtual MenuItems MenuItem { get; set; }

        [ForeignKey("created_by")]
        public virtual Users CreatedByUser { get; set; }

        [ForeignKey("updated_by")]
        public virtual Users UpdatedByUser { get; set; }
    }
}
