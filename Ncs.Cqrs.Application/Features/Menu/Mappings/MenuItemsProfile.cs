using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Features.Menu.DTOs;
using Ncs.Cqrs.Domain.Entities;

namespace Ncs.Cqrs.Application.Features.Menu.Mappings;

public class MenuItemsProfile : Profile
{
    public MenuItemsProfile()
    {
        CreateMap<MenuItems, MenuItemsDto>();

        CreateMap<CreateMenuItemsDto, CreateMenuItemsCommand>();
        CreateMap<UpdateMenuItemsDto, UpdateMenuItemsCommand>();

        CreateMap<CreateMenuItemsCommand, MenuItems>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Ignored during creation
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));


        CreateMap<UpdateMenuItemsCommand, MenuItems>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

    }
}
