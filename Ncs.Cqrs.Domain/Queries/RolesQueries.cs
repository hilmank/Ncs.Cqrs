namespace Ncs.Cqrs.Domain.Queries
{
    public static class RolesQueries
    {
        public const string AllColumns = @"
            roles.id AS ""Id"",
            roles.name AS ""Name""
        ";
    }
}
