using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class CreateUsersCommand : IRequest<ResponseDto<bool>>
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string EmployeeNumber { get; set; }
        public int? CompanyId { get; set; }
        public int? PersonalTypeId { get; set; }
        public string PersonalIdNumber { get; set; }
        public string GuestCompanyName { get; set; }
        public string RfidTag { get; set; }
        public List<int> RolesIds { get; set; }
    }
    public class UsersAddComandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IValidator<CreateUsersCommand> validator) : IRequestHandler<CreateUsersCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<CreateUsersCommand> _validator = validator;
        public async Task<ResponseDto<bool>> Handle(CreateUsersCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            var users = _mapper.Map<Ncs.Cqrs.Domain.Entities.Users>(request);
            var roles = request.RolesIds.Select(x => new Roles { Id = x });

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            users.CreatedBy = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            users.Roles = [.. roles];

            var result = await _unitOfWork.Users.AddUsersAsync(users);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add");
            return ResponseDto<bool>.SuccessResponse(result, "Users added successfully.");
        }
    }

}
