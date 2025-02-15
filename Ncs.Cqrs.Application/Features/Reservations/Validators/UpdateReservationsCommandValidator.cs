using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Reservations.Validators;

public class UpdateReservationsCommandValidator : AbstractValidator<UpdateReservationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReservationsCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Reservation ID must be valid.")
            .MustAsync(ReservationExists).WithMessage("Reservation does not exist or cannot be updated. Only reservations with status Reserved or Canceled can be updated.");

        RuleFor(x => x.MenuItemsId)
            .GreaterThan(0).WithMessage("MenuItemsId must be a valid menu item.")
            .MustAsync(MenuItemExistsAndActive).WithMessage("Selected menu item does not exist or is not active.")
            .MustAsync((command, menuItemsId, cancellationToken) => MenuItemIsAvailableOnSchedule(command, cancellationToken))
            .WithMessage("Selected menu item is not available on the scheduled menu for the reserved date.");

        RuleForEach(x => x.Guests)
            .SetValidator(x => new UpdateReservationGuestsValidator(_unitOfWork, x.Id));
    }

    private async Task<bool> ReservationExists(int reservationId, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(reservationId);

        if (reservation == null)
        {
            return false;
        }

        // Check if the reservation status is Reserved or Canceled
        return reservation.StatusId == ReservationsStatusConstant.Reserved ||
               reservation.StatusId == ReservationsStatusConstant.Canceled;
    }

    private async Task<bool> MenuItemExistsAndActive(int menuItemId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Menus.IsMenuItemActiveAsync(menuItemId);
    }

    private async Task<bool> MenuItemIsAvailableOnSchedule(UpdateReservationsCommand command, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(command.Id);
        if (reservation == null)
            return false;
        return await _unitOfWork.MenuSchedules.IsMenuItemAvailableOnDateAsync(command.MenuItemsId, reservation.ReservedDate);
    }
}
