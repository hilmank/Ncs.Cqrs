namespace Ncs.Cqrs.Domain.Queries
{
    public static class UsersRolesQueries
    {
        public const string AllColumns = @"
            users_roles.user_id AS ""UserId"",
            users_roles.role_id AS ""RoleId""
        ";
    }
}
