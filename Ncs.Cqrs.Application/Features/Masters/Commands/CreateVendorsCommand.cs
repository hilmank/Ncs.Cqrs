using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Masters.Commands
{

    public class CreateVendorsCommand : IRequest<ResponseDto<bool>>
    {
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateVendorsCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IValidator<CreateVendorsCommand> validator)
        : IRequestHandler<CreateVendorsCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<CreateVendorsCommand> _validator = validator;
        public async Task<ResponseDto<bool>> Handle(CreateVendorsCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            var vendors = _mapper.Map<Vendors>(request);
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            vendors.CreatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            var result = await _unitOfWork.Masters.AddVendorAsync(vendors);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add");
            return ResponseDto<bool>.SuccessResponse(result, "Vendors added successfully.");
        }
    }

}
