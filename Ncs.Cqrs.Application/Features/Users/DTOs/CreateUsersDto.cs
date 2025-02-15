using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Ncs.Cqrs.Application.Features.Users.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new user.
    /// </summary>
    public class CreateUsersDto
    {
        [Required]
        [SwaggerSchema("Unique username for the user.")]
        public string Username { get; set; }

        [Required]
        [SwaggerSchema("First name of the user.")]
        public string Firstname { get; set; }

        [SwaggerSchema("Middle name of the user (optional).")]
        public string Middlename { get; set; }

        [Required]
        [SwaggerSchema("Last name of the user.")]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        [SwaggerSchema("Email address of the user.")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [SwaggerSchema("User's phone number.")]
        public string PhoneNumber { get; set; }

        [Required]
        [SwaggerSchema("User's physical address.")]
        public string Address { get; set; }

        [SwaggerSchema("Employee number assigned to the user.")]
        public string EmployeeNumber { get; set; }

        [SwaggerSchema("ID of the company the user is associated with.")]
        public int? CompanyId { get; set; }

        [SwaggerSchema("Type ID of the user's personal classification.")]
        public int? PersonalTypeId { get; set; }

        [SwaggerSchema("Personal identification number of the user.")]
        public string PersonalIdNumber { get; set; }

        [SwaggerSchema("If the user is a guest, the name of the guest's company.")]
        public string GuestCompanyName { get; set; }

        [Required]
        [SwaggerSchema("RFID tag assigned to the user.")]
        public string RfidTag { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be assigned.")]
        [SwaggerSchema("List of role IDs assigned to the user.")]
        public List<int> RolesIds { get; set; }
    }
}
