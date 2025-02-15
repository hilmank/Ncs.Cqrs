using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;

namespace Ncs.Cqrs.Application.Features.Reservations.Validators
{
    public class ChangeReservationsStatusCommandValidator : AbstractValidator<ChangeReservationsStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeReservationsStatusCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Reservation ID must be greater than zero.")
                .MustAsync(ReservationExists).WithMessage("Reservation does not exist.");

            RuleFor(x => x.StatusId)
                .Must(StatusExists).WithMessage("Invalid status provided.")
                .MustAsync((command, statusId, cancellationToken) => StatusChangeIsValid(command, cancellationToken))
                .WithMessage("Invalid status transition.");
        }

        private async Task<bool> ReservationExists(int reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(reservationId);
            return reservation != null;
        }

        private bool StatusExists(int statusId)
        {
            return statusId == ReservationsStatusConstant.Reserved ||
                   statusId == ReservationsStatusConstant.Confirmed ||
                   statusId == ReservationsStatusConstant.Canceled;
        }

        private async Task<bool> StatusChangeIsValid(ChangeReservationsStatusCommand command, CancellationToken cancellationToken)
        {
            var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(command.Id);

            if (reservation == null)
                return false;

            // Check if the status is the same
            if (reservation.StatusId == command.StatusId)
                return false;

            // Only Reserved can transition to Canceled or Confirmed
            if (reservation.StatusId == ReservationsStatusConstant.Reserved)
            {
                return command.StatusId == ReservationsStatusConstant.Canceled ||
                       command.StatusId == ReservationsStatusConstant.Confirmed;
            }

            // If current status is not Reserved, disallow changes
            return false;
        }
    }
}
