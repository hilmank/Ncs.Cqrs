using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Ncs.Cqrs.Infrastructure.Persistence;

public class MenuRepository : IMenuRepository
{

    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    public MenuRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _connection = dbConnectionFactory.CreateConnection();
    }
    private string BaseQuery = $@"
            SELECT 
                {MenuItemsQueries.AllColumns},
                {VendorQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_create.")},
                {UsersQueries.AllColumns.Replace("users.", "users_update.")}
            FROM menu_items
            LEFT JOIN vendors ON menu_items.vendor_id = vendors.id
            JOIN users AS users_create ON menu_items.created_by = users_create.id
            LEFT JOIN users AS users_update ON menu_items.updated_by = users_update.id
            ";
    private async Task<IEnumerable<MenuItems>> QueryMenuItemsAsync(string sql, object? parameters = null)
    {
        var menuDictionary = new Dictionary<int, MenuItems>();

        return await _connection.QueryAsync<MenuItems, Vendors, Users, Users, MenuItems>(
            sql,
            (menuItemns, vendors, userCreate, userUpdate) =>
            {
                if (!menuDictionary.TryGetValue(menuItemns.Id, out var existingMenu))
                {
                    existingMenu = menuItemns;
                    menuDictionary[menuItemns.Id] = existingMenu;
                }
                existingMenu.Vendor = vendors;
                existingMenu.CreatedByUser = userCreate;
                existingMenu.UpdatedByUser = userUpdate;
                return existingMenu;
            },
            parameters,
            splitOn: "Id,Id,Id"
        );
    }

    public async Task<IEnumerable<MenuItems>> GetMenuItemsAsync()
    {
        var sql = $@"
                {BaseQuery}
                WHERE menu_items.is_deleted = @IsDeleted";

        return await QueryMenuItemsAsync(sql, new { IsDeleted = false });
    }
    public async Task<MenuItems?> GetMenuItemsByIdAsync(int menuItemsId)
    {
        var sql = $@"
                {BaseQuery}
                WHERE menu_items.id = @MenuItemsId";
        var menu_items = await QueryMenuItemsAsync(sql, new { MenuItemsId = menuItemsId });
        return menu_items.FirstOrDefault();
    }
    public async Task<int> CountActiveMenuItemsAsync()
    {
        var sql = "SELECT COUNT(*) FROM menu_items WHERE is_active = @IsActive";
        return await _connection.ExecuteScalarAsync<int>(sql, new { IsActive = true });
    }
    public async Task<bool> IsMenuItemActiveAsync(int menuItemId)
    {
        var sql = "SELECT COUNT(1) FROM menu_items WHERE id = @MenuItemId AND is_active = true;";
        var count = await _connection.ExecuteScalarAsync<int>(sql, new { MenuItemId = menuItemId });
        return count > 0;
    }
    public async Task<bool> AddMenuItemsAsync(MenuItems menuItems)
    {
        var affectedRows = await _connection.InsertAsync(menuItems);
        return affectedRows > 0;
    }
    public async Task<bool> UpdateMenuItemsAsync(MenuItems menuItems)
    {
        var affectedRows = await _connection.UpdateAsync(menuItems);
        return affectedRows > 0;

    }
    public async Task<bool> IsMenuNameUniqueAsync(int menuItemsId, string menuName)
    {
        var sql = @"
                SELECT COUNT(*) FROM menu_items 
                WHERE CASE WHEN @MenuItemsId = 0 THEN TRUE ELSE id <> @MenuItemsId END
                AND (LOWER(name) = LOWER(@Name) OR SIMILARITY(LOWER(name), LOWER(@Name)) > 0.5);";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { MenuItemsId = menuItemsId, Name = menuName });
        return count == 0;
    }

    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }
}
