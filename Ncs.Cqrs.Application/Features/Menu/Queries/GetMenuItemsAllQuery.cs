using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Menu.Queries;

public class GetMenuItemsAllQuery : IRequest<ResponseDto<IEnumerable<MenuItemsDto>>>
{

}
public class GetMenuItemsAllHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetMenuItemsAllQuery, ResponseDto<IEnumerable<MenuItemsDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDto<IEnumerable<MenuItemsDto>>> Handle(GetMenuItemsAllQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _unitOfWork.Menus.GetMenuItemsAsync();

        if (menuItems == null || !menuItems.Any())
        {
            return ResponseDto<IEnumerable<MenuItemsDto>>.ErrorResponse(ErrorCodes.NotFound, "Menu items not found");
        }

        var result = _mapper.Map<IEnumerable<MenuItemsDto>>(menuItems);

        return ResponseDto<IEnumerable<MenuItemsDto>>.SuccessResponse(result, "Data retrieved successfully");
    }
}

