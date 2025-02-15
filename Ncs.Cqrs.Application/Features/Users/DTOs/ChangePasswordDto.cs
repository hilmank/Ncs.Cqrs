using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Ncs.Cqrs.Application.Features.Users.DTOs
{
    public class ChangePasswordDto
    {
        [SwaggerSchema(Description = "Username or email of the user requesting the password change.")]
        public string UsernameOrEmail { get; set; }

        [SwaggerSchema(Description = "The current password of the user.")]
        public string OldPassword { get; set; }

        [SwaggerSchema(Description = "The new password to set.")]
        public string NewPassword { get; set; }

        [SwaggerSchema(Description = "Confirmation of the new password.")]
        public string NewPasswordConfirm { get; set; }
    }

    public class ChangePasswordDtoExample : IExamplesProvider<ChangePasswordDto>
    {
        public ChangePasswordDto GetExamples()
        {
            return new ChangePasswordDto
            {
                UsernameOrEmail = "user@example.com",
                OldPassword = "CurrentPassword123",
                NewPassword = "NewSecurePassword456",
                NewPasswordConfirm = "NewSecurePassword456"
            };
        }
    }
}
