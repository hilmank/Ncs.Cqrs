using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ncs.Cqrs.Application.Features.Users.Validators
{
    public class ChangeUsersStatusCommandValidator : AbstractValidator<ChangeUsersStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeUsersStatusCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than zero.");

            RuleFor(x => x.IsActive)
                .MustAsync((command, isActive, cancellationToken) => StatusIsBeingToggled(command.Id, isActive, cancellationToken))
                .WithMessage("The user is already in the requested status.");

            RuleFor(x => x.RfidTag)
                .NotEmpty()
                .When(x => x.IsActive)
                .WithMessage("RFID Tag is required when activating a user.");

            RuleFor(x => x.RfidTag)
                .MustAsync(RfidTagNotExists)
                .When(x => x.IsActive)
                .WithMessage("RFID Tag exists (used) in the database.");
        }

        /// <summary>
        /// Ensures that the user's active status is being toggled.
        /// </summary>
        private async Task<bool> StatusIsBeingToggled(int userId, bool newStatus, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.Users.GetUsersByIdAsync(userId);
            if (existingUser == null)
                return false; // Fails validation if the user does not exist

            return existingUser.IsActive != newStatus; // Ensures the status is actually changing
        }

        /// <summary>
        /// Ensures that the provided RFID tag is not already assigned to another user.
        /// </summary>
        private async Task<bool> RfidTagNotExists(string rfidTag, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(rfidTag))
                return true;

            var exists = await _unitOfWork.Users.ExistsByRfidTagAsync(rfidTag);
            return !exists; // Returns true if the RFID tag is not already in use
        }
    }
}
