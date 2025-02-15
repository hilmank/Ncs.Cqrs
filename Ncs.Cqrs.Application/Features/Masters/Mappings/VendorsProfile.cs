using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Masters.Commands;
using Ncs.Cqrs.Application.Features.Masters.DTOs;
using Ncs.Cqrs.Domain.Entities;

namespace Ncs.Cqrs.Application.Features.Masters.Mappings
{
    public class VendorsProfile : Profile
    {
        public VendorsProfile()
        {
            CreateMap<Vendors, VendorsDto>();

            CreateMap<CreateVendorsDto, CreateVendorsCommand>()
                ;
            CreateMap<UpdateVendorsDto, UpdateVendorsCommand>()
                ;

            CreateMap<CreateVendorsCommand, Vendors>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Ignored during creation
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                ;

            CreateMap<UpdateVendorsCommand, Vendors>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this dynamically in handler
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        }
    }
}
