using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Reservations.Commands
{
    public class ChangeReservationsStatusCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
    }
}

public class ChangeReservationsStatusCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<ChangeReservationsStatusCommand> validator) : IRequestHandler<ChangeReservationsStatusCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeReservationsStatusCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(ChangeReservationsStatusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        var existingReservations = await _unitOfWork.Reservations.GetReservationByIdAsync(request.Id);
        if (existingReservations == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Reservations not found.");
        }

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }

        existingReservations.StatusId = request.StatusId;
        existingReservations.UpdatedBy = int.Parse(userId);
        existingReservations.UpdatedAt = DateTime.UtcNow;

        var result = await _unitOfWork.Reservations.UpdateReservationAsync(existingReservations, existingReservations.Guests);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update reservations");
        return ResponseDto<bool>.SuccessResponse(result, "Reservations updated successfully.");
    }
}
