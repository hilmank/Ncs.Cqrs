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

public class CreateReservationsCommand : IRequest<ResponseDto<bool>> // Returns success status
{
    public int ReservedBy { get; set; } // User ID
    public string ReservedDate { get; set; } // Format: "yyyy-MM-dd"
    public int MenuItemsId { get; set; }
    public bool IsSpicy { get; set; } = false; // Default: Regular (false), Spicy (true)
    public List<CreateReservationGuestsDto> Guests { get; set; } = new List<CreateReservationGuestsDto>();
}

public class CreateReservationsCommandHandler : IRequestHandler<CreateReservationsCommand, ResponseDto<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateReservationsCommand> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateReservationsCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateReservationsCommand> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<bool>> Handle(CreateReservationsCommand request, CancellationToken cancellationToken)
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

        var reservation = _mapper.Map<Domain.Entities.Reservations>(request);
        reservation.CreatedBy = int.Parse(userId);

        var guests = _mapper.Map<List<ReservationGuests>>(request.Guests);

        var result = await _unitOfWork.Reservations.CreateReservationAsync(reservation, guests);

        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add reservations.");

        return ResponseDto<bool>.SuccessResponse(true, "Reservations added successfully.");
    }
}