using Ncs.Cqrs.Application.Features.Users.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Username or Email is required.")
                .MaximumLength(30).WithMessage("Username or Email cannot exceed 30 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
