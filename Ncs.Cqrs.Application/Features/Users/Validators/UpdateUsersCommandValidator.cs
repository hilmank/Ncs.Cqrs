using Ncs.Cqrs.Application.Features.Users.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class UpdateUsersCommandValidator : AbstractValidator<UpdateUsersCommand>
    {
        public UpdateUsersCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than zero.");

            RuleFor(x => x.Firstname)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Firstname))
                .WithMessage("First Name must not exceed 50 characters.");

            RuleFor(x => x.Middlename)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Middlename))
                .WithMessage("Middle Name must not exceed 50 characters.");

            RuleFor(x => x.Lastname)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Lastname))
                .WithMessage("Last Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Invalid email format.")
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Email must not exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Invalid phone number format.");

            RuleFor(x => x.Address)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Address))
                .WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.EmployeeNumber)
                .MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.EmployeeNumber))
                .WithMessage("Employee number must not exceed 30 characters.")
                .NotEmpty().When(x => x.CompanyId.HasValue)
                .WithMessage("Employee Number is required when CompanyId is provided.");

            RuleFor(x => x.CompanyId)
                .GreaterThan(0).When(x => x.CompanyId.HasValue)
                .WithMessage("Company ID must be greater than zero.");

            RuleFor(x => x.PersonalIdNumber)
                .NotEmpty().When(x => x.PersonalTypeId.HasValue)
                .WithMessage("Personal ID Number is required when Personal Type ID is provided.");

            RuleFor(x => x.PersonalTypeId)
                .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.PersonalIdNumber))
                .WithMessage("Personal Type ID is required when Personal ID Number is provided.");

            RuleFor(x => x.GuestCompanyName)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.GuestCompanyName))
                .WithMessage("Guest Company must not exceed 100 characters.");

            RuleFor(x => x.RolesIds)
                .Must(x => x == null || x.All(id => id > 0))
                .WithMessage("All Role IDs must be greater than zero.");
        }
    }
}
