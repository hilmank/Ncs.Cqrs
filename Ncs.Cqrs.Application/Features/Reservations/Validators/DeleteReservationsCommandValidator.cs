using FluentValidation;
using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using System.Threading;
using System.Threading.Tasks;

namespace Ncs.Cqrs.Application.Features.Reservations.Validators;

public class DeleteReservationsCommandValidator : AbstractValidator<DeleteReservationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReservationsCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Reservation ID must be valid.")
            .MustAsync(ReservationExistsAndIsDeletable).WithMessage("Reservation does not exist or cannot be deleted. Only reservations with status Reserved or Canceled can be deleted.");
    }

    private async Task<bool> ReservationExistsAndIsDeletable(int reservationId, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(reservationId);

        if (reservation == null)
        {
            return false; // Reservation does not exist
        }

        // Check if the reservation status is Reserved or Canceled
        return reservation.StatusId == ReservationsStatusConstant.Reserved ||
               reservation.StatusId == ReservationsStatusConstant.Canceled;
    }
}
