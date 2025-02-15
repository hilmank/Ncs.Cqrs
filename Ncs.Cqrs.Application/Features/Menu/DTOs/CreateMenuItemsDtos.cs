using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Application.Features.Menu.DTOs
{
    /// <summary>
    /// Data transfer object for adding a new menu item.
    /// </summary>
    public class CreateMenuItemsDto
    {
        [Required]
        [SwaggerSchema("The vendor ID supplying the menu item.")]
        public int VendorId { get; set; }

        [Required]
        [MaxLength(255)]
        [SwaggerSchema("The name of the menu item.")]
        public string Name { get; set; }

        [Required]
        [SwaggerSchema("A brief description of the menu item.")]
        public string Description { get; set; }

        //[SwaggerSchema("Indicates if the menu item is spicy (true/false).")]
        //public bool IsSpicy { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Calories must be greater than zero.")]
        [SwaggerSchema("The calorie count of the menu item.")]
        public double Calories { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [SwaggerSchema("The price of the menu item.")]
        public double Price { get; set; }

        [Required]
        [SwaggerSchema("The image file of the menu item.")]
        public IFormFile Image { get; set; }
    }
}
