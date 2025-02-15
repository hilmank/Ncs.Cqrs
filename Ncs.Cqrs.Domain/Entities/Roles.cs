using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("roles", Schema = "public")]
    public class Roles
    {
        [Key, Required]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
