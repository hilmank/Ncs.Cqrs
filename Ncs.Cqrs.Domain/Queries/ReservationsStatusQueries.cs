namespace Ncs.Cqrs.Domain.Queries
{
    public static class ReservationsStatusQueries
    {
        public const string AllColumns = @"
            reservations_status.id AS ""Id"",
            reservations_status.name AS ""Name""
        ";
    }
}
