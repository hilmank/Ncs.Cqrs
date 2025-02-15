using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Reservations.Queries
{
    public class GetReservationsByIdQuery : IRequest<ResponseDto<ReservationsDto>>
    {
        public int Id { get; set; }
    }

    public class GetReservationsByIdQueryHandler : IRequestHandler<GetReservationsByIdQuery, ResponseDto<ReservationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReservationsByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<ReservationsDto>> Handle(GetReservationsByIdQuery request, CancellationToken cancellationToken)
        {
            var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(request.Id);

            if (reservation == null)
            {
                return ResponseDto<ReservationsDto>.ErrorResponse(ErrorCodes.NotFound, "Reservation not found.");
            }

            var result = _mapper.Map<ReservationsDto>(reservation);
            return ResponseDto<ReservationsDto>.SuccessResponse(result, "Data retrieved successfully.");
        }
    }
}
