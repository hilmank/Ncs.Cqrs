using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        [SwaggerOperation(Summary = "Authenticates a user and generates a JWT token.", Description = "Authenticates a user using their username/email and password, returning a JWT token.")]
        [SwaggerResponse(200, "User successfully authenticated.", typeof(ResponseDto<SigninDto>))]
        [SwaggerResponse(400, "Invalid login credentials.")]
        [SwaggerResponse(500, "Internal server error.")]
        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<SigninDto>>> UserSignIn([FromBody] UserLoginCommand request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
                return BadRequest(ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, result.MessageDetail));

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user using an RFID card.
        /// </summary>
        [SwaggerOperation(Summary = "Authenticates a user using an RFID card.", Description = "Authenticates a user using their RFID card ID, returning authentication details.")]
        [SwaggerResponse(200, "RFID successfully authenticated.", typeof(ResponseDto<SigninDto>))]
        [SwaggerResponse(400, "Validation failed.")]
        [SwaggerResponse(401, "Unauthorized if the RFID card is not registered.")]
        [HttpPost("rfid-login")]
        public async Task<ActionResult<ResponseDto<SigninDto>>> RfidLogin([FromBody] RfidLoginCommand request)
        {
            var result = await _mediator.Send(request);

            if (!result.Success)
                return BadRequest(ResponseDto<SigninDto>.ErrorResponse(ErrorCodes.InvalidInput, result.MessageDetail));

            return Ok(result);
        }
    }
}
