using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Utils;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class ChangePasswordCommand : IRequest<ResponseDto<bool>>
    {
        public string UsernameOrEmail { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<ChangePasswordCommand> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChangePasswordCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<ChangePasswordCommand> validator,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate request
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
            // Fetch user from database
            var existingUser = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(request.UsernameOrEmail);

            existingUser.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            existingUser.UpdatedBy = int.Parse(userId);
            existingUser.UpdatedAt = DateTime.UtcNow;

            var result = await _unitOfWork.Users.UpdateUsersAsync(existingUser);
            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.UpdateFailed, "Failed to update user");
            return ResponseDto<bool>.SuccessResponse(result, "User updated successfully.");
        }

    }

}
