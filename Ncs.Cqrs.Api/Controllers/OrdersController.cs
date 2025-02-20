using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.MenuSchedule.Queries;
using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Features.Orders.DTOs;
using Ncs.Cqrs.Application.Features.Orders.Queries;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            IMediator mediator,
            IMapper mapper,
            ILogger<MastersController> logger)
            : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("info")]
        [SwaggerOperation(
            Summary = "Get Order Information",
            Description = "Retrieves user information, today's menu items, and reservation details for the authenticated user."
        )]
        public async Task<ActionResult<ResponseDto<OrdersInfoDto>>> GetOrdersInfo()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetOrdersInfoQuery()),
                "Error fetching order information"
            );

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create Orders",
            Description = "Allows the authenticated user to create new orders."
        )]
        public async Task<ActionResult<ResponseDto<bool>>> CreateOrders([FromBody] CreateOrderDto orders)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new CreateOrdersCommand
                {
                    IsSpicy = orders.IsSpicy,
                    MenuItemsId = orders.MenuItemsId,
                    ReservationGuestsIds = orders.ReservationGuestsIds
                }),
                "Error creating order"
            );

        [HttpGet("today")]
        [SwaggerOperation(Summary = "Get current date orders", Description = "Retrieves orders today")]
        public async Task<ActionResult<ResponseDto<IEnumerable<OrdersDto>>>> GetOrdersToday()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetOrdersByDateQuery
                {
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date
                }),
                "Error fetching today's orders"
            );

        [HttpGet("date")]
        [SwaggerOperation(Summary = "Get orders by date", Description = "Retrieves orders by date")]
        public async Task<ActionResult<ResponseDto<IEnumerable<OrdersDto>>>> GetOrdersByDate(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
                startDate = $"{DateTime.Now.Year}-01-01";

            if (string.IsNullOrEmpty(endDate))
                endDate = $"{DateTime.Now.Year}-12-31";

            if (!DateTime.TryParse(startDate, out var parsedStartDate))
                return BadRequest("Invalid start date format. Please provide a valid date (YYYY-MM-DD).");

            if (!DateTime.TryParse(endDate, out var parsedEndDate))
                return BadRequest("Invalid end date format. Please provide a valid date (YYYY-MM-DD).");

            return await HandleRequestAsync(
                async () => await _mediator.Send(new GetOrdersByDateQuery
                {
                    StartDate = parsedStartDate,
                    EndDate = parsedEndDate
                }),
                "Error fetching orders by date"
            );
        }

        [HttpPut("inprocess/{id}")]
        [SwaggerOperation(Summary = "Confirm an order", Description = "Sets the order status to process.")]
        public async Task<ActionResult<ResponseDto<bool>>> ProcessOrder(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.InProcess.ToString() }),
                $"Error processing order with ID {id}"
            );

        [HttpPut("cancel/{id}")]
        [SwaggerOperation(Summary = "Cancel an order", Description = "Sets the order status to canceled.")]
        public async Task<ActionResult<ResponseDto<bool>>> CancelOrder(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.Canceled.ToString() }),
                $"Error canceling order with ID {id}"
            );

        [HttpPut("complete/{id}")]
        [SwaggerOperation(Summary = "Complete an order", Description = "Sets the order status to completed.")]
        public async Task<ActionResult<ResponseDto<bool>>> CompleteOrder(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.Completed.ToString() }),
                $"Error completing order with ID {id}"
            );

        [HttpGet("menu/daily")]
        [SwaggerOperation(Summary = "Get daily menu schedules", Description = "Retrieves all menu schedules for current day.")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuSchedulesDto>>>> GetDailyMenus()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetDailyMenuSchedulesQuery
                {
                    Date = DateTime.Now.Date
                }),
                "Error fetching daily menus"
            );
    }
}
