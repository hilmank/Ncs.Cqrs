using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Commands
{
    public class DeleteMenuSchedulesCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteMenuSchedulesCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteMenuSchedulesCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseDto<bool>> Handle(DeleteMenuSchedulesCommand request, CancellationToken cancellationToken)
        {
            // Check if the menu schedule exists
            var existingMenuSchedule = await _unitOfWork.MenuSchedules.GetMenuSchedulesByIdAsync(request.Id);
            if (existingMenuSchedule == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Menu Schedule not found.");
            }

            // Get the user ID from the token
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }

            // Delete the menu schedule
            var result = await _unitOfWork.MenuSchedules.DeleteMenuSchedulesAsync(request.Id);
            if (!result)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.DeleteFailed, "Failed to delete menu schedule.");
            }

            return ResponseDto<bool>.SuccessResponse(result, "Menu schedule deleted successfully.");
        }
    }
}
