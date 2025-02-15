using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Cqrs.Domain.Entities
{
    [Table("holidays", Schema = "public")]
    public class Holidays
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("holiday_date")]
        public DateTime HolidayDate { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
