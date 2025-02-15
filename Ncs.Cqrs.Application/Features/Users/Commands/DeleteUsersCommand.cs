using System.Security.Claims;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class DeleteUsersCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteUsersCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork
        )
        : IRequestHandler<DeleteUsersCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseDto<bool>> Handle(DeleteUsersCommand request, CancellationToken cancellationToken)
        {
            var currentVendors = await _unitOfWork.Users.GetUsersByIdAsync(request.Id);
            if (currentVendors == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Users not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }

            var result = await _unitOfWork.Users.DeleteUsersAsync(request.Id);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to delete users");
            return ResponseDto<bool>.SuccessResponse(result, "Users deleted successfully.");

        }
    }
}
