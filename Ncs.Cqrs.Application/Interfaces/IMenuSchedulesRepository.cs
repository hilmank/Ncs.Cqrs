using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces
{
    public interface IMenuSchedulesRepository
    {
        void SetTransaction(IDbTransaction transaction);
        Task<MenuSchedules?> GetMenuSchedulesByIdAsync(int id);
        Task<IEnumerable<MenuSchedules>> GetMenuSchedulesDailyAsync(DateTime date);
        Task<IEnumerable<MenuSchedules>> GetMenuSchedulesWeeklyAsync(DateTime startDate);
        Task<IEnumerable<MenuSchedules>> GetMenuSchedulesMonthlyAsync(int year, int month);
        Task<bool> IsMenuItemAvailableOnDateAsync(int menuItemId, DateTime date);
        Task<int> GetMenuCountByDateAsync(DateTime scheduleDate);
        Task<bool> AddMenuSchedulesAsync(List<MenuSchedules> menuSchedules);
        Task<bool> UpdateMenuSchedulesAsync(MenuSchedules menuSchedule);
        Task<bool> DeleteMenuSchedulesAsync(int id);

    }
}
