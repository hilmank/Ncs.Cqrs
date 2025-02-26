using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Domain.Constants;
using FluentValidation;
using MediatR;
using Ncs.Cqrs.Application.Interfaces;

namespace Ncs.Cqrs.Application.Features.Users.Commands
{
    public class RfidLoginCommand : IRequest<ResponseDto<SigninDto>>
    {
        public string RfidTagId { get; set; }
    }
    public class RfidLoginCommandHandler : IRequestHandler<RfidLoginCommand, ResponseDto<SigninDto>>
    {
        private readonly ITokenService _tokenService;
        private readonly IValidator<RfidLoginCommand> _validator;
        private readonly IUnitOfWork _unitOfWork;
        public RfidLoginCommandHandler(IValidator<RfidLoginCommand> validator, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<SigninDto>> Handle(RfidLoginCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            var user = await _unitOfWork.Users.GetUsersByRfidTagAsync(request.RfidTagId);
            var validationError = ValidateUser(user);
            if (!string.IsNullOrEmpty(validationError))
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, validationError);
            }

            var (accessToken, tokenExpirationMinutes) = _tokenService.GenerateAccessToken(user.Id, [.. user.Roles.Select(x => x.Name)]);
            var refreshToken = _tokenService.GenerateRefreshToken();
            SigninDto retVal = new()
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpirationMinutes = tokenExpirationMinutes
            };
            user.RefreshToken = refreshToken;
            user.LastLogin = DateTime.Now;
            _ = await _unitOfWork.Users.UpdateUsersAsync(user);

            return ResponseDto<SigninDto>.SuccessResponse(retVal, "User signed in successfully.");
        }
        private static string ValidateUser(Domain.Entities.Users user)
        {
            if (user == null)
                return "RfidTag Not Registeredl";

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
