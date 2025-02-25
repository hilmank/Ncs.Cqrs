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
    public class ChangeOrdersStatussCommand : IRequest<ResponseDto<bool>>
    {
        public List<int> Ids { get; set; }
        public string Status { get; set; }
    }
}

public class ChangeOrdersStatussCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<ChangeOrdersStatussCommand> validator) : IRequestHandler<ChangeOrdersStatussCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<ChangeOrdersStatussCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(ChangeOrdersStatussCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Retrieve all orders using the list of IDs
        var existingOrders = (await _unitOfWork.Orders.GetOrdersByIdsAsync(request.Ids)).ToList();

        if (existingOrders == null || existingOrders.Count != request.Ids.Count)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "One or more orders not found.");
        }

        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }

        // Update each order
        foreach (var order in existingOrders)
        {
            order.OrderStatus = request.Status;
            order.UpdatedBy = int.Parse(userId);
            order.UpdatedAt = DateTime.Now;
        }

        // Bulk update the orders
        var result = await _unitOfWork.Orders.UpdateOrdersAsync(existingOrders);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update orders.");

        return ResponseDto<bool>.SuccessResponse(result, "Orders updated successfully.");
    }

}
