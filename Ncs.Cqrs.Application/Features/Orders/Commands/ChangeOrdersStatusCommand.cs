using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Orders.Commands
{
    public class ChangeOrdersStatusCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}

public class ChangeOrdersStatusCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<ChangeOrdersStatusCommand> validator) : IRequestHandler<ChangeOrdersStatusCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeOrdersStatusCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(ChangeOrdersStatusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        var existingOrders = await _unitOfWork.Orders.GetOrderByIdAsync(request.Id);
        if (existingOrders == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Orders not found.");
        }

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }

        existingOrders.OrderStatus = request.Status;
        existingOrders.UpdatedBy = int.Parse(userId);
        existingOrders.UpdatedAt = DateTime.Now;

        var result = await _unitOfWork.Orders.UpdateOrderAsync(existingOrders);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update Orders");
        return ResponseDto<bool>.SuccessResponse(result, "Orders updated successfully.");
    }
}
