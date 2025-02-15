using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Ncs.Cqrs.Api.Controllers
{
    [SwaggerResponse(200, description: "Returns the user details.")]
    [SwaggerResponse(400, "Invalid input. Unique value is required.")]
    [SwaggerResponse(401, "Unauthorized request.")]
    [SwaggerResponse(403, "Access denied.")]
    [SwaggerResponse(404, "User not found.")]
    [SwaggerResponse(500, "An error occurred while processing the request.")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseApiController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected string UserId => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        protected string UserName => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        protected string UserEmail => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        protected List<string> UserRoles => _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role)
                                                .Select(c => c.Value)
                                                .ToList() ?? new List<string>();

        protected string UserRolesString => string.Join(",", UserRoles);

        protected ActionResult<ResponseDto<T>> HandleErrorResponse<T>(ResponseDto<T> result)
        {
            if (result == null || result.Success)
                return Ok(result);

            return result.ErrorCode switch
            {
                var code when code == ErrorMessages.GetCode(ErrorCodes.Unauthorized) => Unauthorized(ResponseDto<T>.ErrorResponse(ErrorCodes.Unauthorized, result.MessageDetail)),

                var code when code == ErrorMessages.GetCode(ErrorCodes.AccessDenied) => StatusCode(StatusCodes.Status403Forbidden, ResponseDto<T>.ErrorResponse(ErrorCodes.AccessDenied, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.NotFound) => NotFound(ResponseDto<T>.ErrorResponse(ErrorCodes.NotFound, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.InvalidInput) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.InvalidInput, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.RFIDNotDetected) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.RFIDNotDetected, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.RFIDInvalid) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.RFIDInvalid, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.PaymentFailed) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.PaymentFailed, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.MenuUnavailable) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.MenuUnavailable, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.DatabaseError) => StatusCode(StatusCodes.Status500InternalServerError, ResponseDto<T>.ErrorResponse(ErrorCodes.DatabaseError, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.CreateFailed) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.CreateFailed, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.UpdateFailed) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.UpdateFailed, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.DeleteFailed) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.DeleteFailed, result.MessageDetail)),
                var code when code == ErrorMessages.GetCode(ErrorCodes.DuplicateData) => BadRequest(ResponseDto<T>.ErrorResponse(ErrorCodes.DuplicateData, result.MessageDetail)),

                _ => StatusCode(StatusCodes.Status500InternalServerError, ResponseDto<T>.ErrorResponse(ErrorCodes.UnexpectedError, result.MessageDetail))
            };
        }

    }
}
