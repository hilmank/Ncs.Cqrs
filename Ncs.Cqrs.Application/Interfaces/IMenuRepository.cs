using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces;

public interface IMenuRepository
{
    void SetTransaction(IDbTransaction transaction);
    Task<IEnumerable<MenuItems>> GetMenuItemsAsync();
    Task<MenuItems?> GetMenuItemsByIdAsync(int menuItemsId);
    Task<int> CountActiveMenuItemsAsync();
    Task<bool> IsMenuItemActiveAsync(int menuItemId);
    Task<bool> AddMenuItemsAsync(MenuItems menuItems);
    Task<bool> UpdateMenuItemsAsync(MenuItems menuItems);
    Task<bool> IsMenuNameUniqueAsync(int menuItemsId, string menuName);
}
