using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Reservations.Commands;

public class UpdateReservationsCommand : IRequest<ResponseDto<bool>>
{
    public int Id { get; set; } // Reservation ID to update
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; } = false; // Can be updated
    public List<UpdateReservationGuestsDto> Guests { get; set; } = new List<UpdateReservationGuestsDto>();
}

public class UpdateReservationsCommandHandler : IRequestHandler<UpdateReservationsCommand, ResponseDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateReservationsCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateReservationsCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateReservationsCommand> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<bool>> Handle(UpdateReservationsCommand request, CancellationToken cancellationToken)
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

        var existingReservation = await _unitOfWork.Reservations.GetReservationByIdAsync(request.Id);
        if (existingReservation == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Reservation not found.");
        }

        var updatedReservation = _mapper.Map(request, existingReservation);
        updatedReservation.UpdatedBy = int.Parse(userId);
        updatedReservation.UpdatedAt = DateTime.UtcNow;

        var guests = _mapper.Map<List<ReservationGuests>>(request.Guests);

        var result = await _unitOfWork.Reservations.UpdateReservationAsync(updatedReservation, guests);

        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update reservation.");

        return ResponseDto<bool>.SuccessResponse(true, "Reservation updated successfully.");
    }
}