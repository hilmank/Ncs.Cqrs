using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Features.Orders.DTOs;
using Ncs.Cqrs.Domain.Constants;

namespace Ncs.Cqrs.Application.Features.Orders.Mappings
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            // Entity to DTO
            CreateMap<Domain.Entities.Orders, OrdersDto>();
            CreateMap<Domain.Entities.Users, OrdersInfoUserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                ;
            CreateMap<Domain.Entities.MenuItems, OrdersInfoMenuItemsDto>()
                .ForMember(dest => dest.MenuItemsId, opt => opt.MapFrom(src => src.Id))
                ;
            CreateMap<Domain.Entities.Reservations, OrdersInfoReservationDto>()
                .ForMember(dest => dest.ReservationsId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MenuItem.Name))
                ;
            CreateMap<Domain.Entities.ReservationGuests, ReservationGuestsDto>();
            CreateMap<Domain.Entities.ReservationGuests, OrdersInfoReservationGuestsDto>()
                .ForMember(dest => dest.ReservationGuestsId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MenuItem.Name))
                ;
            CreateMap<Domain.Entities.MenuSchedules, OrdersInfoMenuItemsDto>();

            //Command to Entity
            CreateMap<CreateOrdersCommand, Domain.Entities.Orders>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.Now.Date))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now.Date))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Ignored during creation
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => OrderStatus.Ordered.ToString()))
            ;


        }
    }
}
