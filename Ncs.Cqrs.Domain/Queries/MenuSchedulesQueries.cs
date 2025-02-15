namespace Ncs.Cqrs.Domain.Queries
{
    public static class MenuSchedulesQueries
    {
        public const string AllColumns = @"
            menu_schedules.id AS ""Id"",
            menu_schedules.menu_items_id AS ""MenuItemsId"",
            menu_schedules.available_quantity AS ""AvailableQuantity"",
            menu_schedules.schedule_date AS ""ScheduleDate"",
            menu_schedules.created_at AS ""CreatedAt"",
            menu_schedules.created_by AS ""CreatedBy"",
            menu_schedules.updated_at AS ""UpdatedAt"",
            menu_schedules.updated_by AS ""UpdatedBy""
        ";
    }
}
