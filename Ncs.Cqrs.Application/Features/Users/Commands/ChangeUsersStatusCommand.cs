using System.Security.Claims;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ncs.Cqrs.Application.Features.Users.Commands;

public class ChangeUsersStatusCommand : IRequest<ResponseDto<bool>>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public string RfidTag { get; set; }
}

public class ChangeUsersStatusCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<ChangeUsersStatusCommand> validator) : IRequestHandler<ChangeUsersStatusCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeUsersStatusCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(ChangeUsersStatusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        var existingUser = await _unitOfWork.Users.GetUsersByIdAsync(request.Id);
        if (existingUser == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "User not found.");
        }

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }

        existingUser.IsActive = request.IsActive;
        if (request.IsActive)
        {
            existingUser.RfidTag = request.RfidTag;
            existingUser.IsDeleted = false;
        }
        existingUser.UpdatedBy = int.Parse(userId);
        existingUser.UpdatedAt = DateTime.UtcNow;

        var result = await _unitOfWork.Users.UpdateUsersAsync(existingUser);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update user");
        return ResponseDto<bool>.SuccessResponse(result, "User updated successfully.");
    }
}

