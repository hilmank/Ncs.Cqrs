using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.MenuSchedule.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.Commands
{
    public class CreateMenuSchedulesCommand : IRequest<ResponseDto<bool>>
    {
        public List<MenuItemsAvailableQuantityDto> MenuItems { get; set; }
        public string ScheduleDate { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsMonthly { get; set; }
    }
    public class CreateMenuSchedulesCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IValidator<CreateMenuSchedulesCommand> validator,
        IUnitOfWork unitOfWork,
        IConfiguration configuration) : IRequestHandler<CreateMenuSchedulesCommand, ResponseDto<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConfiguration _configuration = configuration;
        private readonly IValidator<CreateMenuSchedulesCommand> _validator = validator;
        public async Task<ResponseDto<bool>> Handle(CreateMenuSchedulesCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.Unauthorized, "Unauthorized: User ID not found in the token. Please login again.");
            }
            if (!DateTime.TryParse(request.ScheduleDate, out var parsedStartDate))
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, "Invalid date format. Please provide a valid date (YYYY-MM-DD).");
            }
            DateTime today = DateTime.UtcNow.Date;
            DateTime tomorrow = today.AddDays(1);

            if (parsedStartDate < tomorrow)
            {
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.InvalidInput, "You can only schedule for tomorrow or the next day.");
            }

            var holidayDates = await _unitOfWork.Masters.GetHolidaysAsync();
            var schedulesToAdd = new List<MenuSchedules>();
            int maxMenusPerDay = _configuration.GetValue<int>("MenuSettings:MaxMenusPerDay", 2);

            if (request.IsMonthly)
            {
                // Get the total days in the month of the given start date
                int daysInMonth = DateTime.DaysInMonth(parsedStartDate.Year, parsedStartDate.Month);

                for (int i = 0; i < daysInMonth; i++)
                {
                    var newDate = parsedStartDate.AddDays(i);
                    var dayOfWeek = newDate.DayOfWeek;
                    if (holidayDates.Select(x => x.HolidayDate).Contains(newDate) || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                        continue; // Skip holidays and weekends

                    for (int j = 0; j < request.MenuItems.Count; j++)
                    {
                        if (j >= maxMenusPerDay) // only 2 menus per day
                        {
                            break;
                        }
                        schedulesToAdd.Add(new MenuSchedules
                        {
                            MenuItemsId = request.MenuItems[j].MenuItemsId,
                            ScheduleDate = newDate,
                            AvailableQuantity = request.MenuItems[j].AvailableQuantity,
                            CreatedBy = int.Parse(userId),
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
            else if (request.IsWeekly)
            {
                for (int i = 0; i < 7; i++)
                {
                    var newDate = parsedStartDate.AddDays(i);
                    var dayOfWeek = newDate.DayOfWeek;
                    if (holidayDates.Select(x => x.HolidayDate).Contains(newDate) || dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                        continue; // Skip holidays and weekends
                    for (int j = 0; j < request.MenuItems.Count; j++)
                    {
                        if (j >= maxMenusPerDay) // only 2 menus per day
                        {
                            break;
                        }
                        schedulesToAdd.Add(new MenuSchedules
                        {
                            MenuItemsId = request.MenuItems[j].MenuItemsId,
                            ScheduleDate = newDate,
                            AvailableQuantity = request.MenuItems[j].AvailableQuantity,
                            CreatedBy = int.Parse(userId),
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
            else
            {
                for (int i = 0; i < request.MenuItems.Count; i++)
                {
                    schedulesToAdd.Add(new MenuSchedules
                    {
                        MenuItemsId = request.MenuItems[i].MenuItemsId,
                        ScheduleDate = parsedStartDate,
                        AvailableQuantity = request.MenuItems[i].AvailableQuantity,
                        CreatedBy = int.Parse(userId),
                        CreatedAt = DateTime.Now
                    });
                }
            }

            var result = await _unitOfWork.MenuSchedules.AddMenuSchedulesAsync(schedulesToAdd);

            if (!result)
                return ResponseDto<bool>.ErrorResponse(ErrorCodes.CreateFailed, "Failed to add menu schedules.");

            return ResponseDto<bool>.SuccessResponse(true, "Menu schedules added successfully.");
        }
    }
}

