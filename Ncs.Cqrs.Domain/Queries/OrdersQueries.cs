namespace Ncs.Cqrs.Domain.Queries
{
    public static class OrdersQueries
    {
        public const string AllColumns = @"
            orders.id AS ""Id"",
            orders.user_id AS ""UserId"",
            orders.reservation_guests_id AS ""ReservationGuestsId"",
            orders.menu_items_id AS ""MenuItemsId"",
            orders.is_spicy AS ""IsSpicy"",
            orders.reservations_id AS ""ReservationsId"",
            orders.order_type AS ""OrderType"",
            orders.order_status AS ""OrderStatus"",
            orders.order_date AS ""OrderDate"",
            orders.quantity AS ""Quantity"",
            orders.price AS ""Price"",
            orders.created_at AS ""CreatedAt"",
            orders.created_by AS ""CreatedBy"",
            orders.updated_at AS ""UpdatedAt"",
            orders.updated_by AS ""UpdatedBy""
        ";
    }
}
