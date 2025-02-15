using System;
using System.Security.Claims;
using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ncs.Cqrs.Application.Features.Menu.Commands;

public class CreateMenuItemsCommand : IRequest<ResponseDto<bool>>
{
    public int VendorId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Calories { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }
}
public class CreateMenuItemsCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IValidator<CreateMenuItemsCommand> validator)
        : IRequestHandler<CreateMenuItemsCommand, ResponseDto<bool>>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IValidator<CreateMenuItemsCommand> _validator = validator;
    public async Task<ResponseDto<bool>> Handle(CreateMenuItemsCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
        var menuItems = _mapper.Map<MenuItems>(request);
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
        }
        menuItems.CreatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
        var result = await _unitOfWork.Menus.AddMenuItemsAsync(menuItems);
        if (!result)
            return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add");
        return ResponseDto<bool>.SuccessResponse(result, "MenuItems added successfully.");
    }
}
