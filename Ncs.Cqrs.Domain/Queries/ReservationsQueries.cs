namespace Ncs.Cqrs.Domain.Queries
{
    public static class ReservationsQueries
    {
        public const string AllColumns = @"
            reservations.id AS ""Id"",
            reservations.reserved_by AS ""ReservedBy"",
            reservations.reserved_date AS ""ReservedDate"",
            reservations.menu_items_id AS ""MenuItemsId"",
            reservations.is_spicy AS ""IsSpicy"",
            reservations.status_id AS ""StatusId"",
            reservations.created_at AS ""CreatedAt"",
            reservations.created_by AS ""CreatedBy"",
            reservations.updated_at AS ""UpdatedAt"",
            reservations.updated_by AS ""UpdatedBy""
        ";
    }
}
