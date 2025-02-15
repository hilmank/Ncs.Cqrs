using Ncs.Cqrs.Application.Features.Users.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class RfidLoginCommandValidator : AbstractValidator<RfidLoginCommand>
    {
        public RfidLoginCommandValidator()
        {
            RuleFor(x => x.RfidTagId)
                .NotEmpty().WithMessage("Rfid Card is required.")
                .MaximumLength(30).WithMessage("Rfid Card length cannot exceed 30 characters.");
        }
    }
}
