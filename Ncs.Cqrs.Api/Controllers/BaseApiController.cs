using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Ncs.Cqrs.Api.Controllers
{
    [SwaggerResponse(200, "Successful response", typeof(ResponseDto<object>))]
    [SwaggerResponse(400, "Invalid request", typeof(ResponseDto<object>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ResponseDto<object>))]
    [SwaggerResponse(404, "Not Found", typeof(ResponseDto<object>))]
    [SwaggerResponse(500, "Server error", typeof(ResponseDto<object>))]

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private readonly ILogger<BaseApiController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseApiController(IHttpContextAccessor httpContextAccessor, ILogger<BaseApiController> logger)
        {
            _logger = logger;
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
            var statusCode = GetHttpStatusCode(result.ErrorCode);
            var errorCode = ErrorMessages.GetErrorCodeByCode(result.ErrorCode);
            return StatusCode(statusCode, ResponseDto<T>.ErrorResponse(errorCode, result.MessageDetail));
        }
        protected ActionResult<ResponseDto<T>> HandleResponse<T>(ResponseDto<T> result)
        {
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        private int GetHttpStatusCode(string errorCode)
        {
            var errorCodes = ErrorMessages.GetErrorCodeByCode(errorCode);

            return errorCodes switch
            {
                ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorCodes.AccessDenied => StatusCodes.Status403Forbidden,
                ErrorCodes.NotFound => StatusCodes.Status404NotFound,
                ErrorCodes.InvalidInput => StatusCodes.Status400BadRequest,
                ErrorCodes.RFIDNotDetected => StatusCodes.Status400BadRequest,
                ErrorCodes.RFIDInvalid => StatusCodes.Status400BadRequest,
                ErrorCodes.PaymentFailed => StatusCodes.Status400BadRequest,
                ErrorCodes.MenuUnavailable => StatusCodes.Status400BadRequest,
                ErrorCodes.DatabaseError => StatusCodes.Status500InternalServerError,
                ErrorCodes.CreateFailed => StatusCodes.Status400BadRequest,
                ErrorCodes.UpdateFailed => StatusCodes.Status400BadRequest,
                ErrorCodes.DeleteFailed => StatusCodes.Status400BadRequest,
                ErrorCodes.DuplicateData => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }
        protected async Task<ActionResult<ResponseDto<T>>> HandleRequestAsync<T>(Func<Task<ResponseDto<T>>> action, string errorMessage)
        {
            try
            {
                var result = await action();
                return HandleResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);

                return StatusCode(500, ResponseDto<T>.ErrorResponse(
                    ErrorCodes.UnexpectedError,
                    messageDetail: ex.ToString()
                ));
            }
        }

    }
}
