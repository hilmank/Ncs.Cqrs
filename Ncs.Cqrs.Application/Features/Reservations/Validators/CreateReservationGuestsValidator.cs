using Ncs.Cqrs.Application.Features.Reservations.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Reservations.Validators
{
    public class CreateReservationGuestsValidator : AbstractValidator<CreateReservationGuestsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _reservedDate;
        public CreateReservationGuestsValidator(IUnitOfWork unitOfWork, string reservedDate)
        {
            _unitOfWork = unitOfWork;
            _reservedDate = reservedDate;
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("Guest Fullname is required.");

            RuleFor(x => x.PersonalIdNumber)
                .NotEmpty().WithMessage("Personal ID Number is required.");

            RuleFor(x => x.PersonalIdTypeId)
                .GreaterThan(0).WithMessage("Personal ID Type must be a valid type.");

            RuleFor(x => x.MenuItemsId)
                .GreaterThan(0).WithMessage("MenuItemsId must be a valid menu item.")
                .MustAsync(MenuItemExistsAndActive).WithMessage("Selected menu item does not exist or is not active.");

            RuleFor(g => g.MenuItemsId)
                .MustAsync(async (menuItemId, cancellation) =>
                {
                    if (!DateTime.TryParseExact(_reservedDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime reservedDate))
                        return false;
                    return await _unitOfWork.MenuSchedules.IsMenuItemAvailableOnDateAsync(menuItemId, reservedDate);
                })
                .WithMessage("The selected menu item for the guest is not available on the reservation date.");
        }

        private async Task<bool> MenuItemExistsAndActive(int menuItemsId, CancellationToken cancellationToken)
        {
            var menuItem = await _unitOfWork.Menus.GetMenuItemsByIdAsync(menuItemsId);
            return menuItem != null && menuItem.IsActive;
        }

    }
}
