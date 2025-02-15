using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Domain.Entities;

namespace Ncs.Cqrs.Application.Features.Masters.Mappings
{
    public class MastersProfile : Profile
    {
        public MastersProfile()
        {
            CreateMap<Companies, CompaniesDto>();
            CreateMap<PersonalIdType, PersonalIdTypeDto>();
            CreateMap<ReservationsStatus, ReservationsStatusDto>();
            CreateMap<Roles, RolesDto>();

        }
    }
}
