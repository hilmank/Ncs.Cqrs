using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Masters.Commands
{
    public class UpdateVendorsCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class UpdateVendorsCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        IValidator<UpdateVendorsCommand> validator)
        : IRequestHandler<UpdateVendorsCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<UpdateVendorsCommand> _validator = validator;

        public async Task<ResponseDto<bool>> Handle(UpdateVendorsCommand request, CancellationToken cancellationToken)
        {
            var currentVendors = await _unitOfWork.Masters.GetVendorByIdAsync(request.Id);
            if (currentVendors == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "Vendor not found");
            }

            // Update only changed fields
            if (!string.IsNullOrEmpty(request.Name)) currentVendors.Name = request.Name;
            if (!string.IsNullOrEmpty(request.ContactInfo)) currentVendors.ContactInfo = request.ContactInfo;
            if (!string.IsNullOrEmpty(request.PhoneNumber)) currentVendors.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrEmpty(request.Address)) currentVendors.Address = request.Address;
            if (!string.IsNullOrEmpty(request.Email)) currentVendors.Email = request.Email;

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            currentVendors.UpdatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            currentVendors.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.Masters.UpdateVendorAsync(currentVendors);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update vendors");
            return ResponseDto<bool>.SuccessResponse(result, "Vendors updated successfully.");
        }
    }

}
