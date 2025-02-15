using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("companies", Schema = "public")]
    public class Companies
    {
        [Key, Required]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
