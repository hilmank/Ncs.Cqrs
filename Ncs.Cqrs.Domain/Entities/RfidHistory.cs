using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("rfid_history", Schema = "public")]
    public class RfidHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("rfid_tag")]
        public string RfidTag { get; set; }

        [Required]
        [Column("assigned_at")]
        public DateTime AssignedAt { get; set; }

        [Column("unassigned_at")]
        public DateTime? UnassignedAt { get; set; }

        // Navigation property
        [ForeignKey("user_id")]
        public virtual Users User { get; set; }
    }
}
