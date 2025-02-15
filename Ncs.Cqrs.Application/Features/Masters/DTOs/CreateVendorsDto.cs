using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Data transfer object for creating a vendor.
/// </summary>
public class CreateVendorsDto
{
    [Required]
    [SwaggerSchema("The name of the vendor.")]
    public string Name { get; set; }

    [Required]
    [SwaggerSchema("Contact person or department for the vendor.")]
    public string ContactInfo { get; set; }

    [Required]
    [SwaggerSchema("The physical address of the vendor.")]
    public string Address { get; set; }

    [Required]
    [SwaggerSchema("The vendor's phone number.")]
    public string PhoneNumber { get; set; }

    [Required]
    [SwaggerSchema("The vendor's email address.")]
    public string Email { get; set; }
}

