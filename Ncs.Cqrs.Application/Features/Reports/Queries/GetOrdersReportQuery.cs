using System;
using MediatR;
using Ncs.Cqrs.Application.Features.Reports.DTOs;
using Ncs.Cqrs.Application.Interfaces;

namespace Ncs.Cqrs.Application.Features.Reports.Queries;

public class GetOrdersReportQuery : IRequest<byte[]>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public GetOrdersReportQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

}

public class GetOrdersReportQueryHandler : IRequestHandler<GetOrdersReportQuery, byte[]>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IReportService _reportService;

    public GetOrdersReportQueryHandler(IOrdersRepository ordersRepository, IReportService reportService)
    {
        _ordersRepository = ordersRepository;
        _reportService = reportService;
    }

    public async Task<byte[]> Handle(GetOrdersReportQuery request, CancellationToken cancellationToken)
    {
        var orders = await _ordersRepository.GetOrdersByDateAsync(request.StartDate, request.EndDate);
        var orderDtos = orders.Select(o => new OrdersReportDto
        {
            OrderId = o.Id,
            UserName = o.UserOrder.Fullname,
            MenuName = o.MenuItem.Name,
            Quantity = o.Quantity,
            Price = o.Price,
            OrderDate = o.OrderDate
        }).ToList();

        return _reportService.GenerateOrdersReport(orderDtos);
    }
}
