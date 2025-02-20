using System;

namespace Ncs.Cqrs.Application.Features.Reports.DTOs;

public class OrdersReportDto
{
    public int OrderId { get; set; }
    public string UserName { get; set; }
    public string MenuName { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public DateTime OrderDate { get; set; }
}
