using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Users.Queries
{
    public class GetUsersByrfidtTagQuery : IRequest<ResponseDto<UsersDto>>
    {
        public string RfidTag { get; set; }

    }

    public class GetUsersByrfidtTagQueryHandler : IRequestHandler<GetUsersByrfidtTagQuery, ResponseDto<UsersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersByrfidtTagQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<UsersDto>> Handle(GetUsersByrfidtTagQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetUsersByRfidTagAsync(request.RfidTag);

            if (users == null)
            {
                return ResponseDto<UsersDto>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }

            var usersDto = _mapper.Map<UsersDto>(users);

            return ResponseDto<UsersDto>.SuccessResponse(usersDto, "Data retrieved successfully");
        }
    }
}
