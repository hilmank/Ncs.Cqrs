using Ncs.Cqrs.Application.Features.Users.Commands;
using FluentValidation;
using Ncs.Cqrs.Application.Interfaces;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class RfidLoginCommandValidator : AbstractValidator<RfidLoginCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RfidLoginCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(x => x.RfidTagId)
                .NotEmpty().WithMessage("Rfid Card is required.")
                .MaximumLength(30).WithMessage("Rfid Card length cannot exceed 30 characters.");
        }
    }
}
