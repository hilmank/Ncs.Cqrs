﻿using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly IDbConnection _connection; // Renamed from _dbConnection
        private IDbTransaction _transaction;
        private readonly ILogger<OrdersRepository> _logger;
        public OrdersRepository(IDbConnectionFactory dbConnectionFactory, ILogger<OrdersRepository> logger)
        {
            _connection = dbConnectionFactory.CreateConnection();
            _logger = logger;
        }
        private readonly string BaseQuery = $@"
            SELECT 
                {OrdersQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_order.")},
                {MenuItemsQueries.AllColumns},
                {ReservationsQueries.AllColumns},
                {ReservationsStatusQueries.AllColumns},
                {ReservationGuestsQueries.AllColumns},
                {MenuItemsQueries.AllColumns.Replace("menu_items.", "guests_menu_items.")}
            FROM orders
            JOIN users AS users_order ON orders.user_id = users_order.id
            JOIN menu_items ON orders.menu_items_id = menu_items.id
            LEFT JOIN reservations ON orders.reservations_id = reservations.id
            LEFT JOIN reservations_status ON reservations.status_id = reservations_status.id
            LEFT JOIN reservation_guests ON orders.reservation_guests_id = reservation_guests.id
            LEFT JOIN menu_items AS guests_menu_items ON reservation_guests.menu_items_id = guests_menu_items.id
        ";

        private async Task<IEnumerable<Orders>> GetOrdersAsync(string sql, object? parameters = null)
        {
            var resultDictionary = new Dictionary<int, Orders>();

            return await _connection.QueryAsync<Orders, Users, MenuItems, Reservations, ReservationsStatus, ReservationGuests, MenuItems, Orders>(
                sql,
                (order, usersOrder, menuItem, reservation, reservationsStatus, reservationGuest, guestMenuItem) =>
                {
                    if (!resultDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        existingOrder.UserOrder = usersOrder;
                        existingOrder.MenuItem = menuItem;
                        existingOrder.Reservation = reservation;
                        if (reservation is not null)
                        {
                            if (order.ReservationGuestsId is not null)
                            {
                                existingOrder.Reservation.Status = reservationsStatus;
                                existingOrder.ReservationGuests = reservationGuest;
                                existingOrder.ReservationGuests.MenuItem = guestMenuItem;
                            }
                        }
                        resultDictionary[order.Id] = existingOrder;
                    }
                    return existingOrder;
                },
                parameters,
                splitOn: "Id,Id,Id,Id,Id,Id,Id"
            );
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task<IEnumerable<Orders>> GetAllOrdersAsync()
        {
            var sql = $@"
                {BaseQuery}
                ORDER BY orders.order_date DESC";
            var result = await GetOrdersAsync(sql);
            return result.Distinct().ToList();
        }
        public async Task<Orders?> GetOrderByIdAsync(int orderId)
        {
            var sql = $@"
                {BaseQuery}
                WHERE orders.id = @OrderId";
            var result = await GetOrdersAsync(sql, new { OrderId = orderId });
            return result.FirstOrDefault();

        }
        public async Task<IEnumerable<Orders>> GetOrdersByUserIdAsync(int userId)
        {
            var sql = $@"
                {BaseQuery}
                ORDER BY orders.order_date DESC";
            var result = await GetOrdersAsync(sql);
            return result.Distinct().ToList();
        }
        public async Task<IEnumerable<Orders>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate)
        {
            var sql = $@"
                {BaseQuery}
                WHERE orders.order_date BETWEEN @StartDate AND @EndDate
                ORDER BY orders.created_at";
            var result = await GetOrdersAsync(sql, new { StartDate = startDate.Date, EndDate = endDate.Date });
            return result.Distinct().ToList();
        }
        public async Task<bool> CreateOrdersAsync(IEnumerable<Orders> orders)
        {
            var sql = @"
                INSERT INTO orders(user_id, reservation_guests_id, menu_items_id, is_spicy, reservations_id, order_type, order_status, order_date, quantity, price, created_at, created_by)
                VALUES (@UserId, @ReservationGuestsId, @MenuItemsId, @IsSpicy, @ReservationsId, @OrderType, @OrderStatus, @OrderDate, @Quantity, @Price, @CreatedAt, @CreatedBy);"
            ;

            var result = await _connection.ExecuteAsync(sql, orders);
            return result > 0;
        }
        public async Task<bool> UpdateOrderAsync(Orders order)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                var sql = @"
                    UPDATE orders SET 
                      user_id = @UserId, 
                      reservation_guests_id = @ReservationGuestsId, 
                      menu_items_id = @MenuItemsId, 
                      is_spicy = @IsSpicy, 
                      reservations_id = @ReservationsId, 
                      order_type = @OrderType, 
                      order_status = @OrderStatus, 
                      order_date = @OrderDate, 
                      quantity = @Quantity, 
                      price = @Price, 
                      created_at = @CreatedAt, 
                      created_by = @CreatedBy, 
                      updated_at = @UpdatedAt, 
                      updated_by = @UpdatedBy 
                    WHERE
                      id = @Id;
                    ";
                var result = await _connection.ExecuteAsync(sql, order);

                if (order.OrderStatus == OrderStatus.Completed.ToString())
                {
                    sql = @"
                        UPDATE menu_schedules SET 
                          available_quantity = available_quantity - 1
                        WHERE
                          menu_items_id = @MenuItemsId
                        AND schedule_date = @OrderDate;
                        ";
                    _ = await _connection.ExecuteAsync(sql, new { MenuItemsId = order.MenuItemsId, OrderDate = order.OrderDate.Date });
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Failed to update order");
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var sql = @"DELETE FROM orders WHERE id = @Id;";
            var result = await _connection.ExecuteAsync(sql, new { Id = orderId });
            return result > 0;
        }
        public async Task<bool> HasOrderForDateAsync(int userId, DateTime orderDate)
        {
            var sql = $@"
                SELECT COUNT(1) 
                FROM orders 
                WHERE orders.user_id = @UserId
                AND orders.order_date = @OrderDate
                AND orders.order_status != @OrderStatus
                ";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                UserId = userId,
                OrderDate = orderDate.Date,
                OrderStatus = OrderStatus.Canceled.ToString()
            });
            return count > 0;
        }
    }
}
