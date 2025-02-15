using Ncs.Cqrs.Application.Features.Masters.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Masters.Validators
{
    public class UpdateVendorsCommandValidator : AbstractValidator<UpdateVendorsCommand>
    {
        public UpdateVendorsCommandValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(50).WithMessage("Vendor name cannot exceed 50 characters.")
                .When(v => !string.IsNullOrEmpty(v.Name)); // Validate only if provided

            RuleFor(v => v.ContactInfo)
                .MaximumLength(30).WithMessage("Contact information cannot exceed 30 characters.")
                .When(v => !string.IsNullOrEmpty(v.ContactInfo));

            RuleFor(v => v.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
                .When(v => !string.IsNullOrEmpty(v.Address));

            RuleFor(v => v.PhoneNumber)
                .MaximumLength(30).WithMessage("Phone number cannot exceed 30 characters.")
                .When(v => !string.IsNullOrEmpty(v.PhoneNumber));
        }
    }

}
