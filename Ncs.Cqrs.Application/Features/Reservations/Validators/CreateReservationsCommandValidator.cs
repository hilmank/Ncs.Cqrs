using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Features.Reservations.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Reservations.Validators;

public class CreateReservationsCommandValidator : AbstractValidator<CreateReservationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationsCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ReservedBy)
                    .GreaterThan(0).WithMessage("ReservedBy must be a valid user ID.")
                    .MustAsync(UserIsValidForReservation).WithMessage("User must exist, be active, have an RFID tag, and have an Employee or Guest role.");

        RuleFor(x => x.ReservedDate)
            .NotEmpty().WithMessage("ReservedDate is required.")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Invalid date format. Use yyyy-MM-dd.")
            .Must(BeAtLeastOneDayInAdvance).WithMessage("Reservations must be made at least one day in advance.");

        RuleFor(x => x.MenuItemsId)
            .GreaterThan(0).WithMessage("MenuItemsId must be a valid menu item.")
            .MustAsync(MenuItemExistsAndActive).WithMessage("Selected menu item does not exist or is not active.")
            .MustAsync((command, menuItemsId, cancellationToken) => MenuItemIsAvailableOnSchedule(command, cancellationToken))
            .WithMessage("Selected menu item is not available on the scheduled menu for the reserved date.");

        RuleFor(x => x)
            .MustAsync(UserHasNoExistingReservation)
            .WithMessage("User can only make one reservation per day.");

        RuleForEach(x => x.Guests)
            .SetValidator(x => new CreateReservationGuestsValidator(_unitOfWork, x.ReservedDate));
    }
    private async Task<bool> UserIsValidForReservation(int userId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUsersByIdAsync(userId);
        if (user == null || !user.IsActive || string.IsNullOrEmpty(user.RfidTag))
        {
            return false;
        }

        var roleIds = user.Roles.Select(r => r.Id).ToList();
        return roleIds.Contains(RolesConstant.Employee) || roleIds.Contains(RolesConstant.Guest);
    }

    private bool BeAtLeastOneDayInAdvance(string reservedDate)
    {
        if (DateTime.TryParse(reservedDate, out DateTime parsedDate))
        {
            return parsedDate.Date > DateTime.UtcNow.Date;
        }
        return false;
    }

    private async Task<bool> UserHasNoExistingReservation(CreateReservationsCommand command, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(command.ReservedDate, out DateTime reservedDate))
        {
            return false;
        }

        return !await _unitOfWork.Reservations.HasUserReservedForDateAsync(command.ReservedBy, reservedDate);
    }

    private async Task<bool> MenuItemExistsAndActive(int menuItemId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Menus.IsMenuItemActiveAsync(menuItemId);
    }

    private async Task<bool> MenuItemIsAvailableOnSchedule(CreateReservationsCommand command, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(command.ReservedDate, out DateTime parsedDate))
        {
            return false;
        }

        return await _unitOfWork.MenuSchedules.IsMenuItemAvailableOnDateAsync(command.MenuItemsId, parsedDate);
    }
    private async Task<bool> MenuItemsMustBeAvailable(List<CreateReservationGuestsDto> guests, CreateReservationsCommand command, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(command.ReservedDate, out DateTime parsedDate))
        {
            return false;
        }
        foreach (var guest in guests)
        {
            var isAvailable = await _unitOfWork.MenuSchedules.IsMenuItemAvailableOnDateAsync(guest.MenuItemsId, parsedDate);
            if (!isAvailable)
            {
                return false;
            }
        }
        return true;
    }

}
