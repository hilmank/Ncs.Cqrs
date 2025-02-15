namespace Ncs.Cqrs.Domain.Queries
{
    public static class RfidHistoryQueries
    {
        public const string AllColumns = @"
            rfid_history.id AS ""Id"",
            rfid_history.user_id AS ""UserId"",
            rfid_history.rfid_tag AS ""RfidTag"",
            rfid_history.assigned_at AS ""AssignedAt"",
            rfid_history.unassigned_at AS ""UnassignedAt""
        ";
    }
}
