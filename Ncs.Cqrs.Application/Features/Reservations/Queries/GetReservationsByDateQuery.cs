using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Reservations.Queries
{
    public class GetReservationsByDateQuery : IRequest<ResponseDto<IEnumerable<ReservationsDto>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetReservationsByDateQueryHandler : IRequestHandler<GetReservationsByDateQuery, ResponseDto<IEnumerable<ReservationsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReservationsByDateQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<ReservationsDto>>> Handle(GetReservationsByDateQuery request, CancellationToken cancellationToken)
        {
            var reservations = await _unitOfWork.Reservations.GetAllReservationsByDateAsync(request.StartDate, request.EndDate);

            if (reservations == null || reservations.Count == 0)
            {
                return ResponseDto<IEnumerable<ReservationsDto>>.ErrorResponse(ErrorCodes.NotFound, "No reservations found for the specified date range.");
            }

            var result = _mapper.Map<IEnumerable<ReservationsDto>>(reservations);
            return ResponseDto<IEnumerable<ReservationsDto>>.SuccessResponse(result, "Data retrieved successfully.");
        }
    }
}
