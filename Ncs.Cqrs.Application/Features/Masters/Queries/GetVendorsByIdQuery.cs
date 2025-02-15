using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries
{
    public class GetVendorsByIdQuery : IRequest<ResponseDto<VendorsDto>>
    {
        public int VendorId { get; set; }

    }

    public class GetVendorsByIdQueryHandler : IRequestHandler<GetVendorsByIdQuery, ResponseDto<VendorsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetVendorsByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<VendorsDto>> Handle(GetVendorsByIdQuery request, CancellationToken cancellationToken)
        {
            var vendors = await _unitOfWork.Masters.GetVendorByIdAsync(request.VendorId);

            if (vendors == null)
            {
                return ResponseDto<VendorsDto>.ErrorResponse(ErrorCodes.NotFound, "Vendors not found");
            }

            var VendorsDto = _mapper.Map<VendorsDto>(vendors);

            return ResponseDto<VendorsDto>.SuccessResponse(VendorsDto, "Data retrieved successfully");
        }
    }
}
