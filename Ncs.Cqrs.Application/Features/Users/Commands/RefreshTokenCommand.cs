using System;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;

namespace Ncs.Cqrs.Application.Features.Users.Commands;

public class RefreshTokenCommand : IRequest<ResponseDto<SigninDto>>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseDto<SigninDto>>
{
    private readonly ITokenService _tokenService;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(ITokenService tokenService, IValidator<RefreshTokenCommand> validator, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<SigninDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _unitOfWork.Users.GetUsersByRefreshToken(request.RefreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        // Validate access token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        if (principal == null || !principal.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("Invalid access token");

        // Ensure token belongs to the same user
        var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id != userId)
            throw new UnauthorizedAccessException("User ID mismatch");

        // Generate new Access & Refresh Tokens
        var (accessToken, tokenExpirationMinutes) = _tokenService.GenerateAccessToken(user.Id, [.. user.Roles.Select(x => x.Name)]);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Update refresh token in database
        SigninDto retVal = new()
        {
            Token = accessToken,
            RefreshToken = _tokenService.GenerateRefreshToken(),
            TokenExpirationMinutes = tokenExpirationMinutes
        };
        user.RefreshToken = newRefreshToken;
        _ = await _unitOfWork.Users.UpdateUsersAsync(user);
        return ResponseDto<SigninDto>.SuccessResponse(retVal, "Refresh Token in successfully.");
    }
}
