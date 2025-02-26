using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Users.Queries
{
    public class GetUsersByIdQuery : IRequest<ResponseDto<UsersDto>>
    {
        public int? Id { get; set; }

    }

    public class GetUsersByIdQueryHandler : IRequestHandler<GetUsersByIdQuery, ResponseDto<UsersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUsersByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<UsersDto>> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
        {

            if (request.Id == null)
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    request.Id = -1;
                else
                    request.Id = int.Parse(userId);
            }
            var users = await _unitOfWork.Users.GetUsersByIdAsync((int)request.Id);

            if (users == null)
            {
                return ResponseDto<UsersDto>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }

            var usersDto = _mapper.Map<UsersDto>(users);

            return ResponseDto<UsersDto>.SuccessResponse(usersDto, "Data retrieved successfully");
        }
    }
}
