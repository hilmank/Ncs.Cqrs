using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries
{
    public class GetVendorsAllQuery : IRequest<ResponseDto<IEnumerable<VendorsDto>>>
    {
    }
    public class GetVendorsAllHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetVendorsAllQuery, ResponseDto<IEnumerable<VendorsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDto<IEnumerable<VendorsDto>>> Handle(GetVendorsAllQuery request, CancellationToken cancellationToken)
        {
            var vendors = await _unitOfWork.Masters.GetVendorsAsync();

            if (vendors == null || !vendors.Any())
            {
                return ResponseDto<IEnumerable<VendorsDto>>.ErrorResponse(ErrorCodes.NotFound, "Vendors not found");
            }

            var result = _mapper.Map<IEnumerable<VendorsDto>>(vendors);

            return ResponseDto<IEnumerable<VendorsDto>>.SuccessResponse(result, "Data retrieved successfully");
        }
    }
}
