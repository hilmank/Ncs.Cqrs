using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Reservations.Queries
{
    public class GetReservationsByStatusQuery : IRequest<ResponseDto<IEnumerable<ReservationsDto>>>
    {
        public int Status { get; set; }
    }
    public class GetReservationsByStatusQueryHandler : IRequestHandler<GetReservationsByStatusQuery, ResponseDto<IEnumerable<ReservationsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReservationsByStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<ReservationsDto>>> Handle(GetReservationsByStatusQuery request, CancellationToken cancellationToken)
        {
            var reservations = await _unitOfWork.Reservations.GetAllReservationsByStatusAsync(request.Status);

            if (reservations == null || reservations.Count == 0)
            {
                return ResponseDto<IEnumerable<ReservationsDto>>.ErrorResponse(ErrorCodes.NotFound, "No reservations found with the specified status.");
            }

            var result = _mapper.Map<IEnumerable<ReservationsDto>>(reservations);
            return ResponseDto<IEnumerable<ReservationsDto>>.SuccessResponse(result, "Data retrieved successfully.");
        }
    }
}
