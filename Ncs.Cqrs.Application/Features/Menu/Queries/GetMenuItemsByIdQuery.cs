using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Menu.Queries;

public class GetMenuItemsByIdQuery : IRequest<ResponseDto<MenuItemsDto>>
{
    public int Id { get; set; }
}
public class GetMenuItemsByIdQueryHandler : IRequestHandler<GetMenuItemsByIdQuery, ResponseDto<MenuItemsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMenuItemsByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto<MenuItemsDto>> Handle(GetMenuItemsByIdQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _unitOfWork.Menus.GetMenuItemsByIdAsync(request.Id);

        if (vendors == null)
        {
            return ResponseDto<MenuItemsDto>.ErrorResponse(ErrorCodes.NotFound, "MenuItems not found");
        }

        var MenuItemsDto = _mapper.Map<MenuItemsDto>(vendors);

        return ResponseDto<MenuItemsDto>.SuccessResponse(MenuItemsDto, "Data retrieved successfully");
    }
}

