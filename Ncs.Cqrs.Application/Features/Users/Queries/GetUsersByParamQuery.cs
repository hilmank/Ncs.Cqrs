using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Queries
{
    public class GetUsersByParamQuery : IRequest<ResponseDto<IEnumerable<UsersDto>>>
    {
        public int? RolesId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsAcvtive { get; set; }
        public string? Name { get; set; }
    }

    public class GetUsersByParamQueryHandler : IRequestHandler<GetUsersByParamQuery, ResponseDto<IEnumerable<UsersDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersByParamQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<UsersDto>>> Handle(GetUsersByParamQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetUsersByParamsAsync(
                rolesId: request.RolesId,
                companyId: request.CompanyId,
                isActive: request.IsAcvtive,
                name: request.Name
                );

            if (users == null || !users.Any())
            {
                return ResponseDto<IEnumerable<UsersDto>>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }

            var usersDto = _mapper.Map<IEnumerable<UsersDto>>(users);

            return ResponseDto<IEnumerable<UsersDto>>.SuccessResponse(usersDto, "Data retrieved successfully");
        }
    }
}
