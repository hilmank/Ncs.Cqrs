using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Queries
{
    public class GetUsersByIdQuery : IRequest<ResponseDto<UsersDto>>
    {
        public int Id { get; set; }

    }

    public class GetUsersByIdQueryHandler : IRequestHandler<GetUsersByIdQuery, ResponseDto<UsersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<UsersDto>> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetUsersByIdAsync(request.Id);

            if (users == null)
            {
                return ResponseDto<UsersDto>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }

            var usersDto = _mapper.Map<UsersDto>(users);

            return ResponseDto<UsersDto>.SuccessResponse(usersDto, "Data retrieved successfully");
        }
    }
}
