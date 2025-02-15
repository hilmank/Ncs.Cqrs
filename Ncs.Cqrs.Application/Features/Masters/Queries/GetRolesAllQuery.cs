using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries
{
    public class GetRolesAllQuery : IRequest<ResponseDto<IEnumerable<RolesDto>>>
    {
    }
    public class GetRolesAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetRolesAllQuery, ResponseDto<IEnumerable<RolesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDto<IEnumerable<RolesDto>>> Handle(GetRolesAllQuery request, CancellationToken cancellationToken)
        {
            var flavors = await _unitOfWork.Masters.GetRolesAsync();

            if (flavors == null || !flavors.Any())
            {
                return ResponseDto<IEnumerable<RolesDto>>.ErrorResponse(ErrorCodes.NotFound, "Roles not found");
            }

            var result = _mapper.Map<IEnumerable<RolesDto>>(flavors);

            return ResponseDto<IEnumerable<RolesDto>>.SuccessResponse(result, "Data retrieved successfully");
        }
    }
}
