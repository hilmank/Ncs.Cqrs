using System;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Interfaces;

public interface ITokenService
{
    (string token, double tokenExpirationMinutes) GenerateAccessToken(int userId, List<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
