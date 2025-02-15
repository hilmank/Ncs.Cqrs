using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Masters.Commands
{
    public class DeleteVendorsCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteVendorsCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork
        )
        : IRequestHandler<DeleteVendorsCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseDto<bool>> Handle(DeleteVendorsCommand request, CancellationToken cancellationToken)
        {
            var currentVendors = await _unitOfWork.Masters.GetVendorByIdAsync(request.Id);
            if (currentVendors == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Vendor not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            var result = await _unitOfWork.Masters.DeleteVendorAsync(request.Id);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to delete vendors");
            return ResponseDto<bool>.SuccessResponse(result, "vendors deleted successfully.");
        }
    }
}
