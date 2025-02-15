using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Orders.Validators;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Orders.Commands
{
    public class CreateOrdersCommand : IRequest<ResponseDto<bool>>
    {
        public int MenuItemsId { get; set; }

        public bool IsSpicy { get; set; }
        public List<int>? ReservationGuestsIds { get; set; }
    }

    public class CreateOrdersCommandHandler : IRequestHandler<CreateOrdersCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CreateOrdersCommandValidator _validator;

        public CreateOrdersCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _validator = new CreateOrdersCommandValidator(_unitOfWork, httpContextAccessor);
        }

        public async Task<ResponseDto<bool>> Handle(CreateOrdersCommand request, CancellationToken cancellationToken)
        {
            // Get UserId from IHttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            List<Domain.Entities.Orders> orders = new List<Domain.Entities.Orders>();
            var menuItem = await _unitOfWork.Menus.GetMenuItemsByIdAsync(request.MenuItemsId);
            if (menuItem == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Menu item not found.");
            }
            orders.Add(new Domain.Entities.Orders
            {
                UserId = parsedUserId,
                MenuItemsId = request.MenuItemsId,
                IsSpicy = request.IsSpicy,
                OrderDate = DateTime.Now.Date,
                OrderType = request.ReservationGuestsIds == null ? OrderType.WalkIn.ToString() : OrderType.Reservation.ToString(),
                OrderStatus = OrderStatus.Ordered.ToString(),
                Quantity = 1,
                Price = menuItem.Price,
                CreatedBy = parsedUserId,
                CreatedAt = DateTime.Now,
            });
            if (request.ReservationGuestsIds != null)
            {
                for (int i = 0; i < request.ReservationGuestsIds.Count; i++)
                {
                    var guest = await _unitOfWork.Reservations.GetReservationGuestsByidAsync(request.ReservationGuestsIds[i]);
                    if (guest == null) continue;
                    menuItem = await _unitOfWork.Menus.GetMenuItemsByIdAsync(guest.MenuItemsId);
                    if (menuItem == null) continue;
                    orders.Add(new Domain.Entities.Orders
                    {
                        UserId = parsedUserId,
                        MenuItemsId = guest.MenuItemsId,
                        IsSpicy = guest.IsSpicy,
                        OrderDate = DateTime.UtcNow,
                        OrderType = OrderType.Reservation.ToString(),
                        OrderStatus = OrderStatus.Ordered.ToString(),
                        Quantity = 1,
                        Price = menuItem.Price,
                        CreatedBy = parsedUserId,
                        CreatedAt = DateTime.Now,
                        ReservationGuestsId = request.ReservationGuestsIds[i]
                    });
                }
            }

            var result = await _unitOfWork.Orders.CreateOrdersAsync(orders);

            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add orders.");

            return ResponseDto<bool>.SuccessResponse(true, "Orders added successfully.");

        }
    }

}
