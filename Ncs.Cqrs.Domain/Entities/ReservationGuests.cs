using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("reservation_guests", Schema = "public")]
    public class ReservationGuests
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("reservations_id")]
        public int ReservationsId { get; set; }

        [Required]
        [Column("fullname")]
        public string Fullname { get; set; }

        [Column("company_name")]
        public string CompanyName { get; set; }

        [Required]
        [Column("personalid_type_id")]
        public int PersonalIdTypeId { get; set; }

        [Required]
        [Column("personalid_number")]
        public string PersonalIdNumber { get; set; }

        [Required]
        [Column("menu_items_id")]
        public int MenuItemsId { get; set; }

        [Required]
        [Column("is_spicy")]
        public bool IsSpicy { get; set; }

        // Navigation properties
        [ForeignKey("reservations_id")]
        public virtual Reservations Reservation { get; set; }

        [ForeignKey("personalid_type_id")]
        public virtual PersonalIdType PersonalType { get; set; }
        [ForeignKey("menu_items_id")]
        public virtual MenuItems MenuItem { get; set; }
    }
}
