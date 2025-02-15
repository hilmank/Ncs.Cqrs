using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Orders.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Orders.Queries
{
    public class GetOrdersInfoQuery : IRequest<ResponseDto<OrdersInfoDto>>
    {
    }
    public class GetOrdersInfoQueryHandler : IRequestHandler<GetOrdersInfoQuery, ResponseDto<OrdersInfoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetOrdersInfoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<OrdersInfoDto>> Handle(GetOrdersInfoQuery request, CancellationToken cancellationToken)
        {
            // Get UserId from IHttpContextAccessor
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<OrdersInfoDto>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }


            // Fetch user information
            var userEntity = await _unitOfWork.Users.GetUsersByIdAsync(int.Parse(userId));
            if (userEntity == null)
            {
                return ResponseDto<OrdersInfoDto>.ErrorResponse(ErrorCodes.NotFound, "User not found");
            }

            // Fetch today's menu items
            var menuItemsEntities = await _unitOfWork.MenuSchedules.GetMenuSchedulesDailyAsync(DateTime.Now);

            // Fetch reservation information
            var reservationEntity = await _unitOfWork.Reservations.GetUserReservationByDateAsync(int.Parse(userId), DateTime.Now);
            if (reservationEntity == null)
            {
                var hasOrder = await _unitOfWork.Orders.HasOrderForDateAsync(int.Parse(userId), DateTime.Now);
                if (hasOrder)
                    return ResponseDto<OrdersInfoDto>.ErrorResponse(ErrorCodes.InvalidInput, "User can only make one order per day.");
            }
            // Map entities to DTOs
            var userDto = _mapper.Map<OrdersInfoUserDto>(userEntity);
            List<OrdersInfoMenuItemsDto> menuItems = new List<OrdersInfoMenuItemsDto>();
            if (menuItemsEntities != null)
                menuItems = menuItemsEntities.Select(x => _mapper.Map<OrdersInfoMenuItemsDto>(x.MenuItem)).ToList();
            var reservationDto = reservationEntity != null ? _mapper.Map<OrdersInfoReservationDto>(reservationEntity) : null;

            // Assemble the final response
            var ordersInfoDto = new OrdersInfoDto
            {
                User = userDto,
                MenuItems = menuItems,
                Reservation = reservationDto
            };

            return new ResponseDto<OrdersInfoDto>
            {
                Success = true,
                Data = ordersInfoDto
            };
        }
    }
}
