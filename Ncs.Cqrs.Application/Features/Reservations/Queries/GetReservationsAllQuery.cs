using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Reservations.Queries
{
    public class GetReservationsAllQuery : IRequest<ResponseDto<IEnumerable<ReservationsDto>>>
    {
    }

    public class GetReservationsAllQueryHandler : IRequestHandler<GetReservationsAllQuery, ResponseDto<IEnumerable<ReservationsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReservationsAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<ReservationsDto>>> Handle(GetReservationsAllQuery request, CancellationToken cancellationToken)
        {
            var reservations = await _unitOfWork.Reservations.GetAllReservationsAsync();

            if (reservations == null || reservations.Count == 0)
            {
                return ResponseDto<IEnumerable<ReservationsDto>>.ErrorResponse(ErrorCodes.NotFound, "Reservations not found");
            }

            var result = _mapper.Map<IEnumerable<ReservationsDto>>(reservations);
            return ResponseDto<IEnumerable<ReservationsDto>>.SuccessResponse(result, "Data retrieved successfully");
        }
    }

}
