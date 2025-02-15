namespace Ncs.Cqrs.Domain.Queries
{
    public static class VendorQueries
    {
        public const string AllColumns = @"
            vendors.id AS ""Id"",
            vendors.name AS ""Name"",
            vendors.contact_info AS ""ContactInfo"",
            vendors.address AS ""Address"",
            vendors.phone_number AS ""PhoneNumber"",
            vendors.email AS ""Email"",
            vendors.is_active AS ""IsActive"",
            vendors.is_deleted AS ""IsDeleted"",
            vendors.created_at AS ""CreatedAt"",
            vendors.created_by AS ""CreatedBy"",
            vendors.updated_at AS ""UpdatedAt"",
            vendors.updated_by AS ""UpdatedBy""
        ";
    }
}
