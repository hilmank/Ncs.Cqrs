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
    public class UpdateUsersCommand : IRequest<ResponseDto<bool>>
    {
        public int Id { get; set; }
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
    public class UpdateUsersCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IValidator<UpdateUsersCommand> validator)
    : IRequestHandler<UpdateUsersCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<UpdateUsersCommand> _validator = validator;

        public async Task<ResponseDto<bool>> Handle(UpdateUsersCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var existingUser = await _unitOfWork.Users.GetUsersByIdAsync(request.Id);
            if (existingUser == null)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.NotFound, "User not found.");
            }

            // Update only provided fields, keep existing values otherwise
            existingUser.Username = !string.IsNullOrEmpty(request.Username) ? request.Username : existingUser.Username;
            existingUser.Firstname = !string.IsNullOrEmpty(request.Firstname) ? request.Firstname : existingUser.Firstname;
            existingUser.Middlename = !string.IsNullOrEmpty(request.Middlename) ? request.Middlename : existingUser.Middlename;
            existingUser.Lastname = !string.IsNullOrEmpty(request.Lastname) ? request.Lastname : existingUser.Lastname;
            existingUser.Email = !string.IsNullOrEmpty(request.Email) ? request.Email : existingUser.Email;
            existingUser.PhoneNumber = !string.IsNullOrEmpty(request.PhoneNumber) ? request.PhoneNumber : existingUser.PhoneNumber;
            existingUser.Address = !string.IsNullOrEmpty(request.Address) ? request.Address : existingUser.Address;
            existingUser.EmployeeNumber = !string.IsNullOrEmpty(request.EmployeeNumber) ? request.EmployeeNumber : existingUser.EmployeeNumber;
            existingUser.CompanyId = request.CompanyId ?? existingUser.CompanyId;
            existingUser.PersonalTypeId = request.PersonalTypeId ?? existingUser.PersonalTypeId;
            existingUser.PersonalIdNumber = !string.IsNullOrEmpty(request.PersonalIdNumber) ? request.PersonalIdNumber : existingUser.PersonalIdNumber;
            existingUser.GuestCompanyName = !string.IsNullOrEmpty(request.GuestCompanyName) ? request.GuestCompanyName : existingUser.GuestCompanyName;
            existingUser.RfidTag = !string.IsNullOrEmpty(request.RfidTag) ? request.RfidTag : existingUser.RfidTag;

            if (request.RolesIds != null && request.RolesIds.Any())
            {
                existingUser.Roles = request.RolesIds.Select(x => new Roles { Id = x }).ToList();
            }

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            existingUser.UpdatedBy = int.Parse(userId);
            existingUser.UpdatedAt = DateTime.UtcNow;

            var result = await _unitOfWork.Users.UpdateUsersAsync(existingUser);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update user");
            return ResponseDto<bool>.SuccessResponse(result, "User updated successfully.");
        }
    }
}
