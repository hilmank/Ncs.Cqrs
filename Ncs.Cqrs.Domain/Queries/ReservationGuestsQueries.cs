namespace Ncs.Cqrs.Domain.Queries
{
    public static class ReservationGuestsQueries
    {
        public const string AllColumns = @"
            reservation_guests.id AS ""Id"",
            reservation_guests.reservations_id AS ""ReservationsId"",
            reservation_guests.fullname AS ""Fullname"",
            reservation_guests.company_name AS ""CompanyName"",
            reservation_guests.personalid_type_id AS ""PersonalIdTypeId"",
            reservation_guests.personalid_number AS ""PersonalIdNumber"",
            reservation_guests.menu_items_id AS ""MenuItemsId"",
            reservation_guests.is_spicy AS ""IsSpicy""
        ";
    }
}
