using Ncs.Cqrs.Application.Common.DTOs;

namespace Ncs.Cqrs.Application.Features.Users.DTOs
{
    public class SigninDto
    {
        public string Token { get; set; }
        public UsersDto User { get; set; }
    }
}
