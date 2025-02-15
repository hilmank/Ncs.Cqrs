using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Orders.Validators
{
    public class CreateOrdersCommandValidator : AbstractValidator<CreateOrdersCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateOrdersCommandValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

            RuleFor(x => x.MenuItemsId)
                .MustAsync(MenuItemIsScheduledForToday)
                .WithMessage("The selected menu item is not available for today.");

            RuleFor(x => x)
                .MustAsync(UserCanOnlyOrderOncePerDay)
                .WithMessage("You have already placed an order today.");

            When(x => x.ReservationGuestsIds != null && x.ReservationGuestsIds.Count != 0, () =>
            {
                RuleFor(x => x.ReservationGuestsIds)
                    .MustAsync(GuestsBelongToUser)
                    .WithMessage("One or more guests are not registered under your account.");
            });
        }

        private async Task<bool> MenuItemIsScheduledForToday(int menuItemsId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.MenuSchedules.IsMenuItemAvailableOnDateAsync(menuItemsId, DateTime.Now.Date);
        }

        private async Task<bool> UserCanOnlyOrderOncePerDay(CreateOrdersCommand command, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            return !await _unitOfWork.Orders.HasOrderForDateAsync(userId, DateTime.Now.Date);
        }

        private async Task<bool> GuestsBelongToUser(List<int> reservationGuestsIds, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var reservationGuests = await _unitOfWork.Reservations.GetUserReservationByDateAsync(userId, DateTime.Now.Date);

            if (reservationGuests == null)
            {
                return false;
            }

            var reservationGuestIdsFromDb = reservationGuests.Guests.Select(g => g.Id).ToList();

            foreach (var guestId in reservationGuestsIds)
            {
                if (!reservationGuestIdsFromDb.Contains(guestId))
                {
                    return false;
                }
            }

            return true;
        }

        private int GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }
    }
}
