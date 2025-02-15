using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Commands
{
    public class UpdateMenuSchedulesCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public int AvailableQuantity { get; set; }
    }
    public class UpdateMenuSchedulesCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMenuSchedulesCommand> validator)
        : IRequestHandler<UpdateMenuSchedulesCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<UpdateMenuSchedulesCommand> _validator = validator;

        public async Task<ResponseDto<bool>> Handle(UpdateMenuSchedulesCommand request, CancellationToken cancellationToken)
        {
            var currentMenuSchedules = await _unitOfWork.MenuSchedules.GetMenuSchedulesByIdAsync(request.Id);
            if (currentMenuSchedules == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Mneu Schedules not found");
            }

            // Update only changed fields

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            currentMenuSchedules.AvailableQuantity = request.AvailableQuantity;
            currentMenuSchedules.UpdatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            currentMenuSchedules.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.MenuSchedules.UpdateMenuSchedulesAsync(currentMenuSchedules);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update menu schedule");
            return ResponseDto<bool>.SuccessResponse(result, "Menu shedule updated successfully.");
        }
    }

}
