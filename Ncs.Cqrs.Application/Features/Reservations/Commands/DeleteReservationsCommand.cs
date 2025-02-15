using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Reservations.Commands;

public class DeleteReservationsCommand : IRequest<ResponseDto<bool>>
{
    public int Id { get; set; } // Reservation ID to delete
}
public class DeleteReservationsCommandHandler : IRequestHandler<DeleteReservationsCommand, ResponseDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteReservationsCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteReservationsCommandHandler(IUnitOfWork unitOfWork, IValidator<DeleteReservationsCommand> validator, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<bool>> Handle(DeleteReservationsCommand request, CancellationToken cancellationToken)
    {
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

        var reservation = await _unitOfWork.Reservations.GetReservationByIdAsync(request.Id);
        if (reservation == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Reservation not found.");
        }

        var result = await _unitOfWork.Reservations.DeleteReservationAsync(request.Id);

        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.DeleteFailed, "Failed to delete reservation.");

        return ResponseDto<bool>.SuccessResponse(true, "Reservation deleted successfully.");
    }
}
