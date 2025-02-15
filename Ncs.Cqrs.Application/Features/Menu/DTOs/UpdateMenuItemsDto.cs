using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Application.Features.Menu.DTOs
{
    /// <summary>
    /// Data transfer object for updating an existing menu item.
    /// </summary>
    public class UpdateMenuItemsDto
    {
        [SwaggerSchema("The unique identifier of the menu item.")]
        public int Id { get; set; }

        [MaxLength(255)]
        [SwaggerSchema("The updated name of the menu item.")]
        public string Name { get; set; }

        [SwaggerSchema("A brief description of the menu item.")]
        public string Description { get; set; }

        //[SwaggerSchema("Indicates if the menu item is spicy (true/false).")]
        //public bool IsSpicy { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Calories must be greater than zero.")]
        [SwaggerSchema("The calorie count of the menu item.")]
        public double Calories { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [SwaggerSchema("The updated price of the menu item.")]
        public double Price { get; set; }

        [SwaggerSchema("The image file of the menu item.")]
        public IFormFile Image { get; set; }
    }
}
