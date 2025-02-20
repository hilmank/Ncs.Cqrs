using System;
using Ncs.Cqrs.Application.Features.Reports.DTOs;

namespace Ncs.Cqrs.Application.Interfaces;

public interface IReportService
{
    byte[] GenerateOrdersReport(List<OrdersReportDto> orders);
}
