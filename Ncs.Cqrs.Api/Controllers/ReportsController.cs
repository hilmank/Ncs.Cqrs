using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Reports.Queries;
using Ncs.Cqrs.Domain.Constants;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/reports")]
    [ApiVersion("1.0")]
    public class ReportsController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReportsController> logger) : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("orders")]
        [SwaggerOperation(
            Summary = "Generate Orders Report",
            Description = "Fetches order details within a specified date range and returns an Excel report.",
            OperationId = "GetOrdersReport",
            Tags = new[] { "Reports" }
        )]
        public async Task<IActionResult> GetOrdersReport(
            [FromQuery, SwaggerParameter("Start date of the report in YYYY-MM-DD format", Required = true)] string startDate,
            [FromQuery, SwaggerParameter("End date of the report in YYYY-MM-DD format", Required = true)] string endDate)
        {
            try
            {
                if (!DateTime.TryParse(startDate, out var parsedStartDate) ||
                    !DateTime.TryParse(endDate, out var parsedEndDate))
                {

                    return StatusCode(400, ResponseDto<object>.ErrorResponse(ErrorCodes.InvalidInput, "Invalid date format. Please use YYYY-MM-DD."));
                }
                if (parsedEndDate < parsedStartDate)
                {
                    return StatusCode(400, ResponseDto<object>.ErrorResponse(ErrorCodes.InvalidInput, "End date must be greater than or equal to start date."));
                }

                var query = new GetOrdersReportQuery(parsedStartDate, parsedEndDate);
                var fileBytes = await _mediator.Send(query);
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return StatusCode(404, ResponseDto<object>.ErrorResponse(ErrorCodes.NotFound, "No data available for the specified date range."));
                }
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrdersReport.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating orders report.");
                return StatusCode(500, ResponseDto<object>.ErrorResponse(ErrorCodes.UnexpectedError, ex.ToString()));
            }
        }


    }
}
