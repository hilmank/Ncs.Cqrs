using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries;

public class GetReservationsStatusAllQuery : IRequest<ResponseDto<IEnumerable<ReservationsStatusDto>>>
{
}
public class GetReservationsStatusAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetReservationsStatusAllQuery, ResponseDto<IEnumerable<ReservationsStatusDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDto<IEnumerable<ReservationsStatusDto>>> Handle(GetReservationsStatusAllQuery request, CancellationToken cancellationToken)
    {
        var reservationsStatuses = await _unitOfWork.Masters.GetReservationsStatusAsync();

        if (reservationsStatuses == null || !reservationsStatuses.Any())
        {
            return ResponseDto<IEnumerable<ReservationsStatusDto>>.ErrorResponse(ErrorCodes.NotFound, "Reservations status not found");
        }

        var result = _mapper.Map<IEnumerable<ReservationsStatusDto>>(reservationsStatuses);

        return ResponseDto<IEnumerable<ReservationsStatusDto>>.SuccessResponse(result, "Data retrieved successfully");
    }
}

