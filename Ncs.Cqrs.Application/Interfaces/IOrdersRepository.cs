using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces
{
    public interface IOrdersRepository
    {
        void SetTransaction(IDbTransaction transaction);
        Task<IEnumerable<Orders>> GetAllOrdersAsync();
        Task<Orders?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Orders>> GetOrdersByIdsAsync(List<int> ids);
        Task<IEnumerable<Orders>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Orders>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate);
        Task<bool> CreateOrdersAsync(IEnumerable<Orders> order);
        Task<bool> UpdateOrderAsync(Orders order);
        Task<bool> UpdateOrdersAsync(List<Orders> orders);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> HasOrderForDateAsync(int userId, DateTime date);

    }
}
