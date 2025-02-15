using AutoMapper;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Application.Features.Reservations.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Domain.Constants;

namespace Ncs.Cqrs.Application.Features.Reservations.Mappings;

public class ReservationsProfile : Profile
{
    public ReservationsProfile()
    {
        // Entity -> DTO
        CreateMap<Domain.Entities.Reservations, ReservationsDto>()
            .ForMember(dest => dest.ReservedDate, opt => opt.MapFrom(src => src.ReservedDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.Guests, opt => opt.MapFrom(src => src.Guests));

        CreateMap<ReservationGuests, ReservationGuestsDto>();

        // DTO -> Command
        CreateMap<CreateReservationsDto, CreateReservationsCommand>()
            .ForMember(dest => dest.ReservedDate, opt => opt.MapFrom(src => src.ReservedDate))
            .ForMember(dest => dest.Guests, opt => opt.MapFrom(src => src.Guests));

        CreateMap<UpdateReservationsDto, UpdateReservationsCommand>()
            .ForMember(dest => dest.Guests, opt => opt.MapFrom(src => src.Guests));

        // Command -> Entity
        CreateMap<CreateReservationsCommand, Domain.Entities.Reservations>()
            .ForMember(dest => dest.ReservedDate, opt => opt.MapFrom(src => DateTime.Parse(src.ReservedDate)))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => ReservationsStatusConstant.Reserved))
            .ForMember(dest => dest.Guests, opt => opt.Ignore()); // Handle Guests separately

        CreateMap<CreateReservationGuestsDto, ReservationGuests>();

        CreateMap<UpdateReservationsCommand, Domain.Entities.Reservations>()
            .ForMember(dest => dest.Guests, opt => opt.Ignore()); // Handle Guests separately

        CreateMap<UpdateReservationGuestsDto, ReservationGuests>();
    }
}
