using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries
{
    public class GetCompaniesAllQuery : IRequest<ResponseDto<IEnumerable<CompaniesDto>>>
    {
    }
    public class GetCompaniesAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCompaniesAllQuery, ResponseDto<IEnumerable<CompaniesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDto<IEnumerable<CompaniesDto>>> Handle(GetCompaniesAllQuery request, CancellationToken cancellationToken)
        {
            var companies = await _unitOfWork.Masters.GetCompaniesAsync();

            if (companies == null || !companies.Any())
            {
                return ResponseDto<IEnumerable<CompaniesDto>>.ErrorResponse(ErrorCodes.NotFound, "Companies not found");
            }

            var result = _mapper.Map<IEnumerable<CompaniesDto>>(companies);

            return ResponseDto<IEnumerable<CompaniesDto>>.SuccessResponse(result, "Data retrieved successfully");
        }
    }
}
