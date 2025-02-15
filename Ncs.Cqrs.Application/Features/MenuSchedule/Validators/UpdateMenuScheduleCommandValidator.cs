using Ncs.Cqrs.Application.Features.MenuSchedule.Commands;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Validators
{
    public class UpdateMenuScheduleCommandValidator : AbstractValidator<UpdateMenuSchedulesCommand>
    {
        public UpdateMenuScheduleCommandValidator()
        {

            RuleFor(x => x.AvailableQuantity)
                .GreaterThan(0).WithMessage("AvailableQuantity must be greater than zero.");
        }
    }
}