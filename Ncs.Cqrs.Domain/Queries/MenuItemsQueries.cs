namespace Ncs.Cqrs.Domain.Queries
{
    public static class MenuItemsQueries
    {
        public const string AllColumns = @"
            menu_items.id AS ""Id"",
            menu_items.vendor_id AS ""VendorId"",
            menu_items.name AS ""Name"",
            menu_items.description AS ""Description"",
            menu_items.calories AS ""Calories"",
            menu_items.price AS ""Price"",
            menu_items.image_url AS ""ImageUrl"",
            menu_items.is_active AS ""IsActive"",
            menu_items.is_deleted AS ""IsDeleted"",
            menu_items.created_at AS ""CreatedAt"",
            menu_items.created_by AS ""CreatedBy"",
            menu_items.updated_at AS ""UpdatedAt"",
            menu_items.updated_by AS ""UpdatedBy""
        ";
    }
}
