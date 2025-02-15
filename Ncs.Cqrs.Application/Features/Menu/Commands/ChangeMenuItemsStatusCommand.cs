using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Menu.Commands
{
    public class ChangeMenuItemsStatusCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}

public class ChangeMenuItemsStatusCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<ChangeMenuItemsStatusCommand> validator) : IRequestHandler<ChangeMenuItemsStatusCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeMenuItemsStatusCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(ChangeMenuItemsStatusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        var existingMenuItems = await _unitOfWork.Menus.GetMenuItemsByIdAsync(request.Id);
        if (existingMenuItems == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Menu items not found.");
        }

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }

        existingMenuItems.IsActive = request.IsActive;
        if (request.IsActive)
        {
            existingMenuItems.IsDeleted = false;
        }
        existingMenuItems.UpdatedBy = int.Parse(userId);
        existingMenuItems.UpdatedAt = DateTime.UtcNow;

        var result = await _unitOfWork.Menus.UpdateMenuItemsAsync(existingMenuItems);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update menu items");
        return ResponseDto<bool>.SuccessResponse(result, "Menu items updated successfully.");
    }
}