namespace Ncs.Cqrs.Domain.Queries
{
    public static class CompaniesQueries
    {
        public const string AllColumns = @"
            companies.id AS ""Id"",
            companies.name AS ""Name""
        ";
    }
}
