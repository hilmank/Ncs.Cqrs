using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.MenuSchedule.Queries;
using Ncs.Cqrs.Application.Features.Orders.Commands;
using Ncs.Cqrs.Application.Features.Orders.DTOs;
using Ncs.Cqrs.Application.Features.Orders.Queries;
using Ncs.Cqrs.Application.Features.Reservations.Commands;
using Ncs.Cqrs.Application.Features.Reservations.Queries;
using Ncs.Cqrs.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OrdersController(IHttpContextAccessor httpContextAccessor, IMediator mediator, IMapper mapper) : base(httpContextAccessor)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpGet("info")]
        [SwaggerOperation(
          Summary = "Get Order Information",
          Description = "Retrieves user information, today's menu items, and reservation details for the authenticated user.",
          OperationId = "GetOrdersInfo",
          Tags = new[] { "Orders" }
        )]
        public async Task<ActionResult<ResponseDto<OrdersInfoDto>>> GetOrdersInfo()
        {
            var result = await _mediator.Send(new GetOrdersInfoQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        /// <summary> Create new orders for the authenticated user. </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create Orders",
            Description = "Allows the authenticated user to create new orders.",
            OperationId = "CreateOrders",
            Tags = new[] { "Orders" }
        )]
        public async Task<ActionResult<ResponseDto<bool>>> CreateOrders([FromBody] CreateOrderDto orders)
        {
            CreateOrdersCommand command = new()
            {
                IsSpicy = orders.IsSpicy,
                MenuItemsId = orders.MenuItemsId,
                ReservationGuestsIds = orders.ReservationGuestsIds
            };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        [HttpGet("today")]
        [SwaggerOperation(
            Summary = "Get current date orders",
            Description = "Retrieves orders today"
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<OrdersDto>>>> GetOrdersToday()
        {
            var query = new GetOrdersByDateQuery { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.Date };
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        [HttpGet("date")]
        [SwaggerOperation(
            Summary = "Get orders by date",
            Description = "Retrieves orders by date"
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<OrdersDto>>>> GetOrdersByDate(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                startDate = $"{DateTime.Now.Year}-01-01";
            }
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = $"{DateTime.Now.Year}-12-31";
            }
            if (!DateTime.TryParse(startDate, out var parsedStartDate))
            {
                return BadRequest("Invalid start date format. Please provide a valid date (YYYY-MM-DD).");
            }
            if (!DateTime.TryParse(endDate, out var parsedEndDate))
            {
                return BadRequest("Invalid end date format. Please provide a valid date (YYYY-MM-DD).");
            }
            var query = new GetOrdersByDateQuery
            {
                StartDate = parsedStartDate,
                EndDate = parsedEndDate
            };
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        /// <summary> Confirm an order to process. </summary>
        [HttpPut("inprocess/{id}")]
        [SwaggerOperation(Summary = "Confirm an order", Description = "Sets the order status to process.")]
        public async Task<ActionResult<ResponseDto<bool>>> ProcessOrder(int id)
        {
            var command = new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.InProcess.ToString() };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Cancel an order. </summary>
        [HttpPut("cancel/{id}")]
        [SwaggerOperation(Summary = "Cancel an order", Description = "Sets the order status to canceled.")]
        public async Task<ActionResult<ResponseDto<bool>>> CancelOrder(int id)
        {
            var command = new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.Canceled.ToString() };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        /// <summary> Complete an order. </summary>
        [HttpPut("complete/{id}")]
        [SwaggerOperation(Summary = "Complete an order", Description = "Sets the order status to completed.")]
        public async Task<ActionResult<ResponseDto<bool>>> CompleteOrder(int id)
        {
            var command = new ChangeOrdersStatusCommand { Id = id, Status = OrderStatus.Completed.ToString() };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        [HttpGet("menu/daily")]
        [SwaggerOperation(
                    Summary = "Get daily menu schedules",
                    Description = "Retrieves all menu schedules for current day."
                )]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuSchedulesDto>>>> GetDailyMenus()
        {
            var result = await _mediator.Send(new GetDailyMenuSchedulesQuery
            {
                Date = DateTime.Now.Date,
            });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
    }
}
