using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Ncs.Cqrs.Application.Features.Menu.Vlidators
{
    public class ChangeMenuItemsStatusCommandValidator : AbstractValidator<ChangeMenuItemsStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _maxActiveMenuItems;
        public ChangeMenuItemsStatusCommandValidator(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _maxActiveMenuItems = configuration.GetValue<int>("MenuSettings:MaxActiveMenuItems"); // ✅ Read from appsettings.json

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Menu items ID must be greater than zero.");

            RuleFor(x => x.IsActive)
                .MustAsync((command, isActive, cancellationToken) => StatusIsBeingToggled(command.Id, isActive, cancellationToken))
                .WithMessage("The menu items is already in the requested status.");

            RuleFor(x => x)
                .MustAsync(async (command, cancellationToken) => await ValidateActiveMenuItemCount(command.Id, command.IsActive, cancellationToken))
                .WithMessage("Cannot activate more than 2 menu items at the same time.");

        }

        /// <summary>
        /// Ensures that the menuu's active status is being toggled.
        /// </summary>
        private async Task<bool> StatusIsBeingToggled(int menuId, bool newStatus, CancellationToken cancellationToken)
        {
            var existingMenuItems = await _unitOfWork.Menus.GetMenuItemsByIdAsync(menuId);
            if (existingMenuItems == null)
                return false; // Fails validation if the menu item does not exist

            return existingMenuItems.IsActive != newStatus; // Ensures the status is actually changing
        }
        /// <summary>
        /// Validates that no more than 2 menu items are active at the same time.
        /// </summary>
        private async Task<bool> ValidateActiveMenuItemCount(int menuId, bool newStatus, CancellationToken cancellationToken)
        {
            if (!newStatus) return true; // If deactivating, no need to check

            var activeMenuCount = await _unitOfWork.Menus.CountActiveMenuItemsAsync();
            return activeMenuCount < _maxActiveMenuItems; // Allow activation only if fewer than 2 items are active
        }
    }
}
