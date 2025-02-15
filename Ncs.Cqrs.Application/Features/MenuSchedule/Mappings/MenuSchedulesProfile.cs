using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Features.Menu.DTOs;
using Ncs.Cqrs.Application.Features.MenuSchedule.Commands;
using Ncs.Cqrs.Application.Features.MenuSchedule.DTOs;
using Ncs.Cqrs.Domain.Entities;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Mappings
{
    public class MenuSchedulesProfile : Profile
    {
        public MenuSchedulesProfile()
        {
            CreateMap<MenuSchedules, MenuSchedulesDto>();

            CreateMap<CreateMenuSchedulesDto, CreateMenuSchedulesCommand>();
            CreateMap<UpdateMenuSchedulesDto, UpdateMenuSchedulesCommand>();

            CreateMap<CreateMenuSchedulesCommand, MenuSchedules>()
            .ForMember(dest => dest.ScheduleDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Ignored during creation
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                ;


            CreateMap<UpdateMenuSchedulesCommand, MenuSchedules>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        }
    }
}
