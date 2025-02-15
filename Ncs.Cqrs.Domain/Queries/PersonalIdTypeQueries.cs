namespace Ncs.Cqrs.Domain.Queries
{
    public static class PersonalIdTypeQueries
    {
        public const string AllColumns = @"
            personalid_type.id AS ""Id"",
            personalid_type.name AS ""Name""
        ";
    }
}
