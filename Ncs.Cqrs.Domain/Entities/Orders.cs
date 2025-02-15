using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("orders", Schema = "public")]
    public class Orders
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("reservation_guests_id")]
        public int? ReservationGuestsId { get; set; }

        [Required]
        [Column("menu_items_id")]
        public int MenuItemsId { get; set; }

        [Required]
        [Column("is_spicy")]
        public bool IsSpicy { get; set; }

        [Required]
        [Column("reservations_id")]
        public int? ReservationsId { get; set; }

        [Column("order_type")]
        public string OrderType { get; set; }

        [Required]
        [Column("order_status")]
        public string OrderStatus { get; set; }


        [Required]
        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("price")]
        public double Price { get; set; }

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
        [ForeignKey("user_id")]
        public virtual Users UserOrder { get; set; }

        [ForeignKey("reservation_guests_id")]
        public virtual ReservationGuests ReservationGuests { get; set; }

        [ForeignKey("menu_items_id")]
        public virtual MenuItems MenuItem { get; set; }

        [ForeignKey("reservations_id")]
        public virtual Reservations Reservation { get; set; }

        [ForeignKey("created_by")]
        public virtual Users CreatedByUser { get; set; }

        [ForeignKey("updated_by")]
        public virtual Users UpdatedByUser { get; set; }
    }
}
