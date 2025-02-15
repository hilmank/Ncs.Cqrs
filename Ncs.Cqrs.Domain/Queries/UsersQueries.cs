namespace Ncs.Cqrs.Domain.Queries
{
    public static class UsersQueries
    {
        public const string AllColumns = @"
            users.id AS ""Id"",
            users.username AS ""Username"",
            users.password_hash AS ""PasswordHash"",
            users.firstname AS ""FirstName"",
            users.middlename AS ""MiddleName"",
            users.lastname AS ""LastName"",
            users.email AS ""Email"",
            users.phone_number AS ""PhoneNumber"",
            users.address AS ""Address"",
            users.employee_number AS ""EmployeeNumber"",
            users.company_id AS ""CompanyId"",
            users.personalid_type_id AS ""PersonalTypeId"",
            users.personal_id_number AS ""PersonalIdNumber"",
            users.guest_company_name AS ""GuestCompanyName"",
            users.rfid_tag AS ""RfidTag"",
            users.is_active AS ""IsActive"",
            users.is_deleted AS ""IsDeleted"",
            users.created_at AS ""CreatedAt"",
            users.created_by AS ""CreatedBy"",
            users.updated_at AS ""UpdatedAt"",
            users.updated_by AS ""UpdatedBy""
        ";
    }
}
