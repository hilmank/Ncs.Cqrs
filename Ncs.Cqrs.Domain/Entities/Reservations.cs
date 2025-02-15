using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("reservations", Schema = "public")]
    public class Reservations
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("reserved_by")]
        public int ReservedBy { get; set; }

        [Required]
        [Column("reserved_date")]
        public DateTime ReservedDate { get; set; }

        [Required]
        [Column("menu_items_id")]
        public int MenuItemsId { get; set; }

        [Required]
        [Column("is_spicy")]
        public bool IsSpicy { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

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
        [ForeignKey("reserved_by")]
        public virtual Users ReservedByUser { get; set; }

        [ForeignKey("menu_items_id")]
        public virtual MenuItems MenuItem { get; set; }

        [ForeignKey("status_id")]
        public virtual ReservationsStatus Status { get; set; }

        [ForeignKey("created_by")]
        public virtual Users CreatedByUser { get; set; }

        [ForeignKey("updated_by")]
        public virtual Users UpdatedByUser { get; set; }
        public virtual List<ReservationGuests> Guests { get; set; } = new List<ReservationGuests>();
    }
}
