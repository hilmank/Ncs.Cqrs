using FluentValidation;
using Ncs.Cqrs.Application.Features.MenuSchedule.Commands;
using System.Globalization;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Features.MenuSchedule.DTOs;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Validators
{
    /// <summary>
    /// Validator for creating a menu schedule.
    /// </summary>
    public class CreateMenuScheduleCommandValidator : AbstractValidator<CreateMenuSchedulesCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateMenuScheduleCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ScheduleDate)
                .NotEmpty().WithMessage("ScheduleDate is required.")
                .Must(BeAValidDate).WithMessage("ScheduleDate must be a valid date in YYYY-MM-DD format.")
                .Must(BeAFutureDate).WithMessage("ScheduleDate must be a future date.");

            RuleForEach(x => x.MenuItems).SetValidator(new MenuItemsAvailableQuantityDtoValidator(_unitOfWork));
        }

        private bool BeAValidDate(string date)
        {
            return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private bool BeAFutureDate(string date)
        {
            return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                && parsedDate > DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }

    public class MenuItemsAvailableQuantityDtoValidator : AbstractValidator<MenuItemsAvailableQuantityDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuItemsAvailableQuantityDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.MenuItemsId)
                .GreaterThan(0).WithMessage("MenuItemsId must be greater than zero.")
                .MustAsync(MenuItemExists).WithMessage("The specified MenuItemsId does not exist.");

            RuleFor(x => x.AvailableQuantity)
                .GreaterThan(0).WithMessage("Available quantity must be greater than zero.");
        }

        private async Task<bool> MenuItemExists(int menuItemId, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Menus.GetMenuItemsByIdAsync(menuItemId);
            return result != null;
        }
    }
}
