using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Utils;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or email is required.")
                .MaximumLength(100).WithMessage("Username or email must not exceed 100 characters.")
                .MustAsync(UserExists).WithMessage("User does not exist.");

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MustAsync(ValidateOldPassword).WithMessage("Incorrect current password.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("New password must contain at least one number.")
                .Matches(@"[\W]").WithMessage("New password must contain at least one special character.")
                .NotEqual(x => x.OldPassword).WithMessage("New password cannot be the same as the current password.");

            RuleFor(x => x.NewPasswordConfirm)
                .Equal(x => x.NewPassword).WithMessage("New password confirmation does not match.");
        }

        private async Task<bool> UserExists(string usernameOrEmail, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(usernameOrEmail);
            return user != null;
        }

        private async Task<bool> ValidateOldPassword(ChangePasswordCommand dto, string oldPassword, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(dto.UsernameOrEmail);
            if (user == null) return false;

            return PasswordHasher.VerifyPassword(oldPassword, user.PasswordHash);

        }

    }
}
