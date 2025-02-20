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

        public ReservationsController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<MastersController> logger,
            IMediator mediator,
            IMapper mapper)
            : base(httpContextAccessor, logger)
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
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetReservationsAllQuery()),
                "Error fetching all reservations"
            );

        [HttpGet("status/{status}")]
        [SwaggerOperation(
            Summary = "Get reservations by status",
            Description = "Retrieves reservations filtered by the provided status code."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsDto>>>> GetReservationsByStatus(int status)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetReservationsByStatusQuery { Status = status }),
                $"Error fetching reservations with status {status}"
            );

        [HttpGet("date")]
        [SwaggerOperation(
            Summary = "Get reservations by date range",
            Description = "Retrieves reservations within the specified date range. The dates should be in 'yyyy-MM-dd' format."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsDto>>>> GetReservationsByDate([FromQuery] string startDate, [FromQuery] string endDate)
        {
            if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
                return BadRequest("Invalid start date format. Use 'yyyy-MM-dd'.");

            if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end))
                return BadRequest("Invalid end date format. Use 'yyyy-MM-dd'.");

            return await HandleRequestAsync(
                async () => await _mediator.Send(new GetReservationsByDateQuery { StartDate = start, EndDate = end }),
                $"Error fetching reservations between {start:yyyy-MM-dd} and {end:yyyy-MM-dd}"
            );
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get reservation by ID",
            Description = "Retrieves a specific reservation based on the provided reservation ID."
        )]
        public async Task<ActionResult<ResponseDto<ReservationsDto>>> GetReservationById(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetReservationsByIdQuery { Id = id }),
                $"Error fetching reservation with ID {id}"
            );

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new reservation",
            Description = "Creates a new reservation with optional guest information."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> CreateReservation([FromBody] CreateReservationsDto reservations)
            => await HandleRequestAsync(
                async () => await _mediator.Send(_mapper.Map<CreateReservationsCommand>(reservations)),
                "Error creating reservation"
            );

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing reservation",
            Description = "Updates reservation details and guest information for a specific reservation ID."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateReservation(int id, [FromBody] UpdateReservationsDto updateDto)
            => await HandleRequestAsync(
                async () =>
                {
                    var command = _mapper.Map<UpdateReservationsCommand>(updateDto);
                    command.Id = id;
                    return await _mediator.Send(command);
                },
                $"Error updating reservation with ID {id}"
            );

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a reservation",
            Description = "Deletes a specific reservation by its ID."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteReservation(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new DeleteReservationsCommand { Id = id }),
                $"Error deleting reservation with ID {id}"
            );

        [HttpPut("confirm/{id}")]
        [SwaggerOperation(Summary = "Confirm a reservation", Description = "Sets the reservation status to confirmed.")]
        public async Task<ActionResult<ResponseDto<bool>>> ConfirmReservation(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeReservationsStatusCommand { Id = id, StatusId = ReservationsStatusConstant.Confirmed }),
                $"Error confirming reservation with ID {id}"
            );

        [HttpPut("cancel/{id}")]
        [SwaggerOperation(Summary = "Cancel a reservation", Description = "Sets the reservation status to canceled.")]
        public async Task<ActionResult<ResponseDto<bool>>> CancelReservation(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeReservationsStatusCommand { Id = id, StatusId = ReservationsStatusConstant.Canceled }),
                $"Error canceling reservation with ID {id}"
            );
    }
}
