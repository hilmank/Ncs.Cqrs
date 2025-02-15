using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.Queries;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Orders.Queries
{
    public class GetOrdersByDateQuery : IRequest<ResponseDto<IEnumerable<OrdersDto>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetOrdersByDateQueryHandler : IRequestHandler<GetOrdersByDateQuery, ResponseDto<IEnumerable<OrdersDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersByDateQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<OrdersDto>>> Handle(GetOrdersByDateQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByDateAsync(request.StartDate, request.EndDate);

            if (orders == null || orders.Count() == 0)
            {
                return ResponseDto<IEnumerable<OrdersDto>>.ErrorResponse(ErrorCodes.NotFound, "No orders found for the specified date range.");
            }

            var result = _mapper.Map<IEnumerable<OrdersDto>>(orders);
            return ResponseDto<IEnumerable<OrdersDto>>.SuccessResponse(result, "Data retrieved successfully.");
        }
    }
}
