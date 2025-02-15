using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Queries
{
    public class GetUsersByUsernameOrEmailQuery : IRequest<ResponseDto<UsersDto>>
    {
        public string UsernameOrEmail { get; set; }

    }

    public class GetUsersByUsernameOrEmailQueryHandler : IRequestHandler<GetUsersByUsernameOrEmailQuery, ResponseDto<UsersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersByUsernameOrEmailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<UsersDto>> Handle(GetUsersByUsernameOrEmailQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(request.UsernameOrEmail);

            if (users == null)
            {
                return ResponseDto<UsersDto>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }

            var usersDto = _mapper.Map<UsersDto>(users);

            return ResponseDto<UsersDto>.SuccessResponse(usersDto, "Data retrieved successfully");
        }
    }
}
