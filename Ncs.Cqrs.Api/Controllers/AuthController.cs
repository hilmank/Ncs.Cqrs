using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Ncs.Cqrs.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator, IHttpContextAccessor httpContextAccessor, ILogger<MastersController> logger)
            : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Authenticates a user and generates a JWT token.",
            Description = "Authenticates a user using their username/email and password, returning a JWT token."
        )]
        [SwaggerResponse(200, "User successfully authenticated.", typeof(ResponseDto<SigninDto>))]
        [SwaggerResponse(400, "Invalid login credentials.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<ActionResult<ResponseDto<SigninDto>>> UserSignIn([FromBody] UserLoginCommand request)
            => await HandleRequestAsync(
                async () => await _mediator.Send(request),
                "Error during user login"
            );

        /// <summary>
        /// Authenticates a user using an RFID card.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("rfid-login")]
        [SwaggerOperation(
            Summary = "Authenticates a user using an RFID card.",
            Description = "Authenticates a user using their RFID card ID, returning authentication details."
        )]
        [SwaggerResponse(200, "RFID successfully authenticated.", typeof(ResponseDto<SigninDto>))]
        [SwaggerResponse(400, "Validation failed.")]
        [SwaggerResponse(401, "Unauthorized if the RFID card is not registered.")]
        public async Task<ActionResult<ResponseDto<SigninDto>>> RfidLogin([FromBody] RfidLoginCommand request)
            => await HandleRequestAsync(
                async () => await _mediator.Send(request),
                "Error during RFID login"
            );
    }
}
