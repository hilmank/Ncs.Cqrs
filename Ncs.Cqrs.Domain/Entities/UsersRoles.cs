using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("users_roles", Schema = "public")]
    public class UsersRoles
    {
        [Key, Required]
        [Column("user_Id")]
        public int UserId { get; set; }

        [Key, Required]
        [Column("role_id")]
        public int RoleId { get; set; }

        // Navigation properties
        [ForeignKey("user_Id")]
        public virtual Users User { get; set; }

        [ForeignKey("role_id")]
        public virtual Roles Role { get; set; }
    }
}
