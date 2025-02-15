using System.Security.Claims;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ncs.Cqrs.Application.Features.Menu.Commands;

public class UpdateMenuItemsCommand : IRequest<ResponseDto<bool>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Calories { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }
}
public class UpdateMenuItemsCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMenuItemsCommand> validator)
        : IRequestHandler<UpdateMenuItemsCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<UpdateMenuItemsCommand> _validator = validator;

    public async Task<ResponseDto<bool>> Handle(UpdateMenuItemsCommand request, CancellationToken cancellationToken)
    {
        var currentMenuItems = await _unitOfWork.Menus.GetMenuItemsByIdAsync(request.Id);
        if (currentMenuItems == null)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "MenuItems not found");
        }

        // Update only changed fields
        if (!string.IsNullOrEmpty(request.Name)) currentMenuItems.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description)) currentMenuItems.Description = request.Description;
        if (request.Calories > 0) currentMenuItems.Calories = request.Calories;
        if (request.Price > 0) currentMenuItems.Price = request.Price;
        if (!string.IsNullOrEmpty(request.ImageUrl)) currentMenuItems.ImageUrl = request.ImageUrl;

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
        currentMenuItems.UpdatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
        currentMenuItems.UpdatedAt = DateTime.Now;

        var result = await _unitOfWork.Menus.UpdateMenuItemsAsync(currentMenuItems);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update menu items");
        return ResponseDto<bool>.SuccessResponse(result, "Menu items updated successfully.");
    }
}
