using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Cqrs.Application.Features.Orders.DTOs
{
    public class CreateOrderDto
    {
        //[Required]
        //[SwaggerSchema("The unique identifier of the user placing the order.")]
        //public int UserId { get; set; }
        [Required]
        [SwaggerSchema("The unique identifier of the menu item being ordered.")]
        public int MenuItemsId { get; set; }

        [Required]
        [SwaggerSchema("Indicates if the menu item is the spicy variant.")]
        public bool IsSpicy { get; set; }
        [SwaggerSchema("The unique guest Ids of the reservation, if applicable.")]
        public List<int>? ReservationGuestsIds { get; set; }
    }
}
