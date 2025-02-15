using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Menu.Queries;

public class GetMenuSchedulesByIdQuery : IRequest<ResponseDto<MenuSchedulesDto>>
{
    public int Id { get; set; }
}
public class GetMenuSchedulesByIdQueryHandler : IRequestHandler<GetMenuSchedulesByIdQuery, ResponseDto<MenuSchedulesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMenuSchedulesByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto<MenuSchedulesDto>> Handle(GetMenuSchedulesByIdQuery request, CancellationToken cancellationToken)
    {
        var menuSchedules = await _unitOfWork.MenuSchedules.GetMenuSchedulesByIdAsync(request.Id);

        if (menuSchedules == null)
        {
            return ResponseDto<MenuSchedulesDto>.ErrorResponse(ErrorCodes.NotFound, "MenuItems not found");
        }

        var result = _mapper.Map<MenuSchedulesDto>(menuSchedules);

        return ResponseDto<MenuSchedulesDto>.SuccessResponse(result, "Data retrieved successfully");
    }
}

