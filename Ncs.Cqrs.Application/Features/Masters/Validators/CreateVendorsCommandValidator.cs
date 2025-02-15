using Ncs.Cqrs.Application.Features.Masters.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Masters.Validators
{
    public class CreateVendorsCommandValidator : AbstractValidator<CreateVendorsCommand>
    {
        public CreateVendorsCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Vendor name is required.")
                .MaximumLength(50).WithMessage("Vendor name cannot exceed 50 characters.");

            RuleFor(v => v.ContactInfo)
                .NotEmpty().WithMessage("Contact information is required.")
                .MaximumLength(30).WithMessage("Contact information cannot exceed 30 characters.");

            RuleFor(v => v.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

            RuleFor(v => v.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(30).WithMessage("Phone number cannot exceed 30 characters.");

        }
    }
}
