using System;
using Ncs.Cqrs.Application.Features.Reports.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using NPOI.XSSF.UserModel;

namespace Ncs.Cqrs.Infrastructure.Persistence;

public class ReportService : IReportService
{
    private readonly string _templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OrderReportTemplate.xlsx");

    public byte[] GenerateOrdersReport(List<OrdersReportDto> orders)
    {
        using var templateStream = new FileStream(_templatePath, FileMode.Open, FileAccess.Read);
        using var workbook = new XSSFWorkbook(templateStream);
        var sheet = workbook.GetSheetAt(0); // Assuming data starts on the first sheet

        int startRow = 1; // Assuming data starts from row 2 (0-based index)
        foreach (var order in orders)
        {
            var row = sheet.CreateRow(startRow++);
            row.CreateCell(0).SetCellValue(order.OrderId);
            row.CreateCell(1).SetCellValue(order.UserName);
            row.CreateCell(2).SetCellValue(order.MenuName);
            row.CreateCell(3).SetCellValue(order.Quantity);
            row.CreateCell(4).SetCellValue((double)order.Price);
            row.CreateCell(5).SetCellValue(order.OrderDate.ToString("yyyy-MM-dd"));
        }

        using var outputStream = new MemoryStream();
        workbook.Write(outputStream);
        return outputStream.ToArray();
    }
}