using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using System.Data;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class MenuSchedulesRepository : IMenuSchedulesRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public MenuSchedulesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _connection = dbConnectionFactory.CreateConnection();
        }
        private string BaseQuery = $@"
            SELECT 
                {MenuSchedulesQueries.AllColumns},
                {MenuItemsQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_create.")},
                {UsersQueries.AllColumns.Replace("users.", "users_update.")}
            FROM menu_schedules
            LEFT JOIN menu_items ON menu_schedules.menu_items_id = menu_items.id
            JOIN users AS users_create ON menu_schedules.created_by = users_create.id
            LEFT JOIN users AS users_update ON menu_schedules.updated_by = users_update.id
            ";
        private async Task<IEnumerable<MenuSchedules>> QueryMenuSchedulesAsync(string sql, object? parameters = null)
        {
            var menuDictionary = new Dictionary<int, MenuSchedules>();

            return await _connection.QueryAsync<MenuSchedules, MenuItems, Users, Users, MenuSchedules>(
                sql,
                (menuSchedules, menuItems, userCreate, userUpdate) =>
                {
                    if (!menuDictionary.TryGetValue(menuSchedules.Id, out var existingMenuSchedule))
                    {
                        existingMenuSchedule = menuSchedules;
                        menuDictionary[menuSchedules.Id] = existingMenuSchedule;
                    }
                    existingMenuSchedule.MenuItem = menuItems;
                    existingMenuSchedule.CreatedByUser = userCreate;
                    existingMenuSchedule.UpdatedByUser = userUpdate;
                    return existingMenuSchedule;
                },
                parameters,
                splitOn: "Id,Id,Id"
            );
        }
        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
        public async Task<MenuSchedules?> GetMenuSchedulesByIdAsync(int id)
        {
            var sql = $@"
                {BaseQuery}
                WHERE menu_schedules.id = @Id";
            var menuSchedule = await QueryMenuSchedulesAsync(sql, new { Id = id });
            return menuSchedule.FirstOrDefault();
        }
        public async Task<IEnumerable<MenuSchedules>> GetMenuSchedulesDailyAsync(DateTime date)
        {
            var sql = $@"
                {BaseQuery}
                WHERE menu_schedules.schedule_date = @Date";
            return await QueryMenuSchedulesAsync(sql, new { Date = date.Date });
        }
        public async Task<IEnumerable<MenuSchedules>> GetMenuSchedulesWeeklyAsync(DateTime startDate)
        {
            var sql = $@"
                {BaseQuery}
                WHERE menu_schedules.schedule_date >= @StartDate
                AND menu_schedules.schedule_date < @EndDate";
            var endDateTime = startDate.AddDays(7);
            return await QueryMenuSchedulesAsync(sql, new { StartDate = startDate, EndDate = endDateTime });
        }
        public async Task<IEnumerable<MenuSchedules>> GetMenuSchedulesMonthlyAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);
            var sql = $@"
                {BaseQuery}
                WHERE menu_schedules.schedule_date >= @StartDate
                AND menu_schedules.schedule_date < @EndDate";
            return await QueryMenuSchedulesAsync(sql, new { StartDate = startDate, EndDate = endDate });
        }
        public async Task<bool> IsMenuItemAvailableOnDateAsync(int menuItemId, DateTime date)
        {
            var sql = @"
                SELECT COUNT(1) FROM menu_schedules 
                WHERE menu_items_id = @MenuItemId AND schedule_date = @Date;";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { MenuItemId = menuItemId, Date = date.Date });
            return count > 0;
        }
        public async Task<int> GetMenuCountByDateAsync(DateTime scheduleDate)
        {
            var query = "SELECT COUNT(*) FROM menu_schedules WHERE schedule_date = @ScheduleDate";
            var count = await _connection.ExecuteScalarAsync<int>(query, new { ScheduleDate = scheduleDate });
            return count;
        }
        public async Task<bool> AddMenuSchedulesAsync(List<MenuSchedules> menuSchedules)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();
            try
            {
                // Extract unique dates from the incoming data
                var scheduledDates = menuSchedules.Select(ms => ms.ScheduleDate).Distinct().ToList();

                // Delete existing records for the given dates
                const string deleteSql = "DELETE FROM menu_schedules WHERE schedule_date = ANY(@ScheduledDates);";
                await _connection.ExecuteAsync(deleteSql, new { ScheduledDates = scheduledDates }, transaction);

                // Insert new menu schedules
                foreach (var item in menuSchedules)
                {
                    var affectedRows = await _connection.InsertAsync(item, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }


        public async Task<bool> UpdateMenuSchedulesAsync(MenuSchedules menuSchedule)
        {
            var affectedRows = await _connection.UpdateAsync(menuSchedule);
            return affectedRows > 0;
        }
        public async Task<bool> DeleteMenuSchedulesAsync(int id)
        {
            var query = "DELETE FROM menu_schedules WHERE id = @Id;";
            var affectedRows = await _connection.ExecuteAsync(query, new { Id = id });
            return affectedRows > 0;
        }
    }
}
