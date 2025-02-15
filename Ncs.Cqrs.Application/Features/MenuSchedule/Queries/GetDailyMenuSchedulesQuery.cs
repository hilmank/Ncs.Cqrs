using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Queries
{
    public class GetDailyMenuSchedulesQuery : IRequest<ResponseDto<IEnumerable<MenuSchedulesDto>>>
    {
        public DateTime Date { get; set; }
    }
    public class GetDailyMenuSchedulesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetDailyMenuSchedulesQuery, ResponseDto<IEnumerable<MenuSchedulesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDto<IEnumerable<MenuSchedulesDto>>> Handle(GetDailyMenuSchedulesQuery request, CancellationToken cancellationToken)
        {
            var menuItems = await _unitOfWork.MenuSchedules.GetMenuSchedulesDailyAsync(request.Date);

            if (menuItems == null || !menuItems.Any())
            {
                return ResponseDto<IEnumerable<MenuSchedulesDto>>.ErrorResponse(ErrorCodes.NotFound, "Menu schedules not found");
            }

            var result = _mapper.Map<IEnumerable<MenuSchedulesDto>>(menuItems);

            return ResponseDto<IEnumerable<MenuSchedulesDto>>.SuccessResponse(result, "Data retrieved successfully");
        }
    }
}

