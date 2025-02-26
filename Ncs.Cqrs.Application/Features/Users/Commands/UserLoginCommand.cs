using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Services;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Utils;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class UserLoginCommand : IRequest<ResponseDto<SigninDto>>
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ResponseDto<SigninDto>>
    {
        private readonly ITokenService _tokenService;
        private readonly IValidator<UserLoginCommand> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public UserLoginCommandHandler(
            IValidator<UserLoginCommand> validator,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<SigninDto>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            // Validate user input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<SigninDto>.ErrorResponse(
                    ErrorCodes.InvalidInput,
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // Fetch user from the database
            var user = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(request.UsernameOrEmail);
            var validationError = ValidateUser(user, request.Password);
            if (!string.IsNullOrEmpty(validationError))
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, validationError);
            }

            // Generate tokens
            var (accessToken, tokenExpirationMinutes) = _tokenService.GenerateAccessToken(
                user.Id, user.Roles.Select(x => x.Name).ToList());

            var refreshToken = _tokenService.GenerateRefreshToken();

            // Update user with new refresh token & last login time
            user.RefreshToken = refreshToken;
            user.LastLogin = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateUsersAsync(user);

            // Create response object
            var response = new SigninDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpirationMinutes = tokenExpirationMinutes
            };

            return ResponseDto<SigninDto>.SuccessResponse(response, "User signed in successfully.");
        }

        /// <summary>
        /// Validates user existence and credentials.
        /// </summary>
        private static string ValidateUser(Ncs.Cqrs.Domain.Entities.Users user, string password)
        {
            if (user == null)
                return "Invalid username or email";

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return "Wrong password";

            if (!user.IsActive)
                return "User is inactive";

            if (user.IsDeleted)
                return "User is deleted";

            if (user.Roles?.Any() != true)
                return "User has no role";

            return string.Empty; // No validation errors
        }
    }
}
