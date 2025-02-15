using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Features.Reservations.DTOs;
using Ncs.Cqrs.Application.Features.Reservations.Queries;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/reservations")]
    public class ReservationsController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ReservationsController(IHttpContextAccessor httpContextAccessor, IMediator mediator, IMapper mapper) : base(httpContextAccessor)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all reservations",
            Description = "Retrieves all reservations from the database."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsDto>>>> GetAllReservations()
        {
            var query = new GetReservationsAllQuery();
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpGet("status/{status}")]
        [SwaggerOperation(
            Summary = "Get reservations by status",
            Description = "Retrieves reservations filtered by the provided status code."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsDto>>>> GetReservationsByStatus(int status)
        {
            var query = new GetReservationsByStatusQuery { Status = status };
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpGet("date")]
        [SwaggerOperation(
            Summary = "Get reservations by date range",
            Description = "Retrieves reservations within the specified date range. The dates should be in 'yyyy-MM-dd' format."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsDto>>>> GetReservationsByDate([FromQuery] string startDate, [FromQuery] string endDate)
        {
            if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
            {
                return BadRequest("Invalid start date format. Use 'yyyy-MM-dd'.");
            }

            if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end))
            {
                return BadRequest("Invalid end date format. Use 'yyyy-MM-dd'.");
            }

            var query = new GetReservationsByDateQuery { StartDate = start, EndDate = end };
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get reservation by ID",
            Description = "Retrieves a specific reservation based on the provided reservation ID."
        )]
        public async Task<ActionResult<ResponseDto<ReservationsDto>>> GetReservationById(int id)
        {
            var query = new GetReservationsByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new reservation",
            Description = "Creates a new reservation with optional guest information."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> CreateReservation([FromBody] CreateReservationsDto reservations)
        {
            var command = _mapper.Map<CreateReservationsCommand>(reservations);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing reservation",
            Description = "Updates reservation details and guest information for a specific reservation ID."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateReservation(int id, [FromBody] UpdateReservationsDto updateDto)
        {
            var command = _mapper.Map<UpdateReservationsCommand>(updateDto);
            command.Id = id;
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a reservation",
            Description = "Deletes a specific reservation by its ID."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteReservation(int id)
        {
            var command = new DeleteReservationsCommand { Id = id };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        /// <summary> Confirm a reservation. </summary>
        [HttpPut("confirm/{id}")]
        [SwaggerOperation(Summary = "Confirm a reservation", Description = "Sets the reservation status to confirmed.")]
        public async Task<ActionResult<ResponseDto<bool>>> ConfirmReservation(int id)
        {
            var command = new ChangeReservationsStatusCommand { Id = id, StatusId = ReservationsStatusConstant.Confirmed };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Cancel a reservation. </summary>
        [HttpPut("cancel/{id}")]
        [SwaggerOperation(Summary = "Cancel a reservation", Description = "Sets the reservation status to canceled.")]
        public async Task<ActionResult<ResponseDto<bool>>> CancelReservation(int id)
        {
            var command = new ChangeReservationsStatusCommand { Id = id, StatusId = ReservationsStatusConstant.Canceled };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
    }
}
