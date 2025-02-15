using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Utils;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ncs.Cqrs.Application.Services
{
    public interface IAuthService
    {
        Task<ResponseDto<SigninDto>> AuthenticateAsync(string usernameOrEmail, string password);
        Task<ResponseDto<SigninDto>> AuthenticateAsync(string rfidCardId);
    }
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper) : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private string GenerateJwtToken(Users user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating JWT token.");

            var jwtSettings = _configuration.GetSection("JwtSettings");

            // Use null-coalescing operator to handle potential null values
            string secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing in appsettings.json.");
            string issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing in appsettings.json.");
            string audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is missing in appsettings.json.");
            string expiryMinutesStr = jwtSettings["ExpiryMinutes"] ?? throw new InvalidOperationException("JWT ExpiryMinutes is missing in appsettings.json.");

            if (!double.TryParse(expiryMinutesStr, out double expiryMinutes))
                throw new InvalidOperationException("Invalid ExpiryMinutes value in JWT settings.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString() ?? throw new InvalidOperationException("User has no userid.")),
                new(ClaimTypes.Surname, user.Fullname ?? throw new InvalidOperationException("User has no fullname.")),
                new(ClaimTypes.Name, user.Username ?? throw new InvalidOperationException("User has no username.")),
                new(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User has no email.")),
            };

            if (user.Roles?.Any() == true)
            {
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Id.ToString())));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ResponseDto<SigninDto>> AuthenticateAsync(string usernameOrEmail, string password)
        {
            //string sss = "Bsi4dm1n2025";
            //string bbb = PasswordHasher.HashPassword(sss);
            var user = await _unitOfWork.Users.GetUsersByUsernameOrEmailAsync(usernameOrEmail);
            string errMessage = string.Empty;
            if (user == null)
            {
                errMessage = "Invalid username or email";
            }
            else if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                errMessage = "Wrong password";
            }
            else if (!user.IsActive)
            {
                errMessage = "User is inactive";
            }
            else if (user.IsDeleted)
            {
                errMessage = "User is deleted";
            }
            else if (user.Roles?.Any() != true)
            {
                errMessage = "User has no role";
            }

            if (!string.IsNullOrEmpty(errMessage))
            {
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, errMessage);

            }
            else
            {
                SigninDto retVal = new()
                {
                    Token = GenerateJwtToken(user),
                    User = _mapper.Map<UsersDto>(user),
                };
                return ResponseDto<SigninDto>.SuccessResponse(retVal, "User signed in successfully.");
            }
        }

        public async Task<ResponseDto<SigninDto>> AuthenticateAsync(string rfidCardId)
        {
            var user = await _unitOfWork.Users.GetUsersByRfidTagAsync(rfidCardId);
            if (user == null)
                return ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, "RFID not registered");
            SigninDto retVal = new()
            {
                Token = GenerateJwtToken(user),
                User = _mapper.Map<UsersDto>(user),
            };
            return ResponseDto<SigninDto>.SuccessResponse(retVal, "User signed in successfully.");
        }
    }

}
