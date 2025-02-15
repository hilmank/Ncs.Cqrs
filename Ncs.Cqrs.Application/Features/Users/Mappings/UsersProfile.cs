using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Utils;
using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Features.Users.Mappings
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            _ = CreateMap<Ncs.Cqrs.Domain.Entities.Users, UsersDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) // Format CreatedDate
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.HasValue
                    ? src.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : null))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles ?? new List<Roles>()))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.PersonalIdType, opt => opt.MapFrom(src => src.PersonalIdType))
                ;

            CreateMap<CreateUsersDto, CreateUsersCommand>()
                ;
            CreateMap<UpdateUsersDto, UpdateUsersCommand>()
                ;

            CreateMap<ChangePasswordDto, ChangePasswordCommand>()
                ;

            CreateMap<CreateUsersCommand, Ncs.Cqrs.Domain.Entities.Users>()
                        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Assuming users are active by default
                        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                        .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Set this in the service based on logged-in user
                        .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                        .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                        .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => PasswordHasher.HashPassword("Bsi2025")))
                        ;

            CreateMap<UpdateUsersCommand, Ncs.Cqrs.Domain.Entities.Users>()
                        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                        .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                        ;
        }
    }
}
