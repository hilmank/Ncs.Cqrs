using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class CreateUsersCommandValidator : AbstractValidator<CreateUsersCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUsersCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .MustAsync(async (username, cancellation) => !await _unitOfWork.Users.ExistsByUsernameAsync(username))
                .WithMessage("Username already exists.");

            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name must not exceed 50 characters.");

            RuleFor(x => x.Middlename)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Middlename))
                .WithMessage("Middle Name must not exceed 50 characters.");

            RuleFor(x => x.Lastname)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Lastname))
                .WithMessage("Last Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .MustAsync(async (email, cancellation) => !await _unitOfWork.Users.ExistsByEmailAsync(email))
                .WithMessage("Email already exists.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.EmployeeNumber)
                .NotEmpty().When(x => x.CompanyId.HasValue)
                .WithMessage("Employee Number is required when CompanyId is provided.")
                .MaximumLength(30).WithMessage("Employee number must not exceed 30 characters.");

            RuleFor(x => x.CompanyId)
                .NotNull().When(x => string.IsNullOrEmpty(x.GuestCompanyName))
                .WithMessage("Company ID is required when Guest Company is not provided.")
                .GreaterThan(0).When(x => x.CompanyId.HasValue)
                .WithMessage("Company ID must be greater than zero.");

            RuleFor(x => x.PersonalIdNumber)
                .NotEmpty().When(x => !x.CompanyId.HasValue)
                .WithMessage("Personal ID is required when CompanyId is not provided.");

            RuleFor(x => x.PersonalTypeId)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.PersonalIdNumber))
                .WithMessage("Personal ID Type is required when Personal ID Number is provided.");

            RuleFor(x => x.GuestCompanyName)
                .NotEmpty().When(x => !x.CompanyId.HasValue)
                .WithMessage("Guest Company is required when Company ID is not provided.")
                .MaximumLength(100).WithMessage("Guest Company must not exceed 100 characters.");

            RuleFor(x => x.RfidTag)
                .NotEmpty().WithMessage("RFID Tag is required.")
                .MustAsync(async (rfid, cancellation) => !await _unitOfWork.Users.ExistsByRfidTagAsync(rfid))
                .WithMessage("RFID Tag already exists.");

            RuleFor(x => x.RolesIds)
                .NotEmpty().WithMessage("At least one Role ID is required.")
                .Must(x => x.All(id => id > 0)).WithMessage("All Role IDs must be greater than zero.");
        }
    }
}
