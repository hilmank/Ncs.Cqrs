using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Ncs.Cqrs.Infrastructure.Persistence;

public class ReservationsRepository : IReservationsRepository
{
    private readonly IDbConnection _connection; // Renamed from _dbConnection
    private IDbTransaction _transaction;
    private readonly ILogger<ReservationsRepository> _logger;
    public ReservationsRepository(IDbConnectionFactory dbConnectionFactory, ILogger<ReservationsRepository> logger)
    {
        _connection = dbConnectionFactory.CreateConnection();
        _logger = logger;
    }
    private string BaseQuery = $@"
            SELECT 
                {ReservationsQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_reserved.")},
                {MenuItemsQueries.AllColumns},
                {ReservationsStatusQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_create.")},
                {ReservationGuestsQueries.AllColumns},
                {MenuItemsQueries.AllColumns.Replace("menu_items.", "guests_menu_items.")}
            FROM reservations
            JOIN users AS users_reserved ON reservations.reserved_by = users_reserved.id
            JOIN menu_items ON reservations.menu_items_id = menu_items.id
            JOIN reservations_status ON reservations.status_id = reservations_status.id
            JOIN users AS users_create ON reservations.created_by = users_create.id
            LEFT JOIN reservation_guests ON reservations.id = reservation_guests.reservations_id
            LEFT JOIN menu_items AS guests_menu_items ON reservation_guests.menu_items_id = guests_menu_items.id
            ";
    private async Task<IEnumerable<Reservations>> QueryReservationsAsync(string sql, object? parameters = null)
    {
        var resultDictionary = new Dictionary<int, Reservations>();

        return await _connection.QueryAsync<Reservations, Users, MenuItems, ReservationsStatus, Users, ReservationGuests, MenuItems, Reservations>(
            sql,
            (reservations, userReserve, menuItems, reservationStatus, userCreate, guest, guestMenu) =>
            {
                if (!resultDictionary.TryGetValue(reservations.Id, out var existingReservation))
                {
                    existingReservation = reservations;
                    existingReservation.Guests = new List<ReservationGuests>();
                    resultDictionary[reservations.Id] = existingReservation;

                    // Assign related data
                    existingReservation.ReservedByUser = userReserve;
                    existingReservation.MenuItem = menuItems;
                    existingReservation.Status = reservationStatus;
                    existingReservation.CreatedByUser = userCreate;
                }

                if (guest != null && !existingReservation.Guests.Any(g => g.Id == guest.Id))
                {
                    guest.MenuItem = guestMenu;
                    (existingReservation.Guests as List<ReservationGuests>)?.Add(guest);
                }

                return existingReservation;
            },
            parameters,
            splitOn: "Id,Id,Id,Id,Id,Id"
        );
    }

    public void SetTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }
    public async Task<bool> CreateReservationAsync(Reservations reservation, List<ReservationGuests> guests)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();
        using var transaction = _connection.BeginTransaction();
        try
        {
            var sql = @"
                INSERT INTO reservations(reserved_by, reserved_date, menu_items_id, is_spicy, status_id, created_at, created_by)
                VALUES (@ReservedBy, @ReservedDate, @MenuItemsId, @IsSpicy, @StatusId, NOW(), @CreatedBy)
                RETURNING id;";

            var reservationId = await _connection.ExecuteScalarAsync<int>(sql, reservation, transaction);

            if (guests != null && guests.Any())
            {
                foreach (var guest in guests)
                {
                    guest.ReservationsId = reservationId;
                    var guestSql = @"
                        INSERT INTO reservation_guests(reservations_id, fullname, company_name, personalid_type_id, personalid_number, menu_items_id, is_spicy)
                        VALUES (@ReservationsId, @Fullname, @CompanyName, @PersonalIdTypeId, @PersonalIdNumber, @MenuItemsId, @IsSpicy);";

                    await _connection.ExecuteAsync(guestSql, guest, transaction);
                }
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to create reservation for user {@ReservedBy} on {@ReservedDate}", reservation.ReservedBy, reservation.ReservedDate);
            return false;
        }
        finally
        {
            _connection.Close();
        }

    }

    public async Task<bool> UpdateReservationAsync(Reservations reservation, List<ReservationGuests> guests)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        using var transaction = _connection.BeginTransaction();
        try
        {
            var sql = @"
                UPDATE reservations 
                SET menu_items_id = @MenuItemsId, is_spicy = @IsSpicy, status_id = @StatusId, updated_at = NOW(), updated_by = @UpdatedBy 
                WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, reservation, transaction);

            // Delete existing guests
            var deleteGuestSql = "DELETE FROM reservation_guests WHERE reservations_id = @Id;";
            await _connection.ExecuteAsync(deleteGuestSql, new { reservation.Id }, transaction);

            // Insert new guests
            if (guests != null && guests.Any())
            {
                foreach (var guest in guests)
                {
                    guest.ReservationsId = reservation.Id;
                    var guestSql = @"
                        INSERT INTO reservation_guests(reservations_id, fullname, company_name, personalid_type_id, personalid_number, menu_items_id, is_spicy)
                        VALUES (@ReservationsId, @Fullname, @CompanyName, @PersonalIdTypeId, @PersonalIdNumber, @MenuItemsId, @IsSpicy);";

                    await _connection.ExecuteAsync(guestSql, guest, transaction);
                }
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to update reservation with ID {@Id} on {@ReservedDate}", reservation.Id, reservation.ReservedDate);
            return false;
        }
        finally
        {
            _connection.Close();
        }

    }

    public async Task<bool> DeleteReservationAsync(int reservationId)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();
        using var transaction = _connection.BeginTransaction();
        try
        {
            // Delete guests first
            var deleteGuestsSql = "DELETE FROM reservation_guests WHERE reservations_id = @ReservationId;";
            await _connection.ExecuteAsync(deleteGuestsSql, new { ReservationId = reservationId }, transaction);

            // Delete reservation
            var sql = "DELETE FROM reservations WHERE id = @ReservationId;";
            await _connection.ExecuteAsync(sql, new { ReservationId = reservationId }, transaction);

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to delete reservation with ID {@ReservationId}", reservationId);
            return false;
        }
        finally
        {
            _connection.Close();
        }

    }

    public async Task<Reservations?> GetReservationByIdAsync(int reservationId)
    {
        var sql = $@"
                {BaseQuery}
                WHERE reservations.id = @ReservationId";
        var result = await QueryReservationsAsync(sql, new { ReservationId = reservationId });
        return result.FirstOrDefault();

    }

    public async Task<List<Reservations>> GetAllReservationsAsync()
    {
        var sql = $@"
                {BaseQuery}
                ORDER BY reservations.reserved_date DESC";
        var result = await QueryReservationsAsync(sql);
        return result.Distinct().ToList();
    }
    public async Task<List<Reservations>> GetAllReservationsByDateAsync(DateTime startDate, DateTime endDate)
    {
        var sql = $@"
                {BaseQuery}
                WHERE reservations.reserved_date BETWEEN @StartDate AND @EndDate
                ORDER BY reservations.reserved_date DESC";
        var result = await QueryReservationsAsync(sql, new { StartDate = startDate.Date, EndDate = endDate.Date });
        return result.Distinct().ToList();
    }
    public async Task<List<Reservations>> GetAllReservationsByStatusAsync(int status)
    {
        var sql = $@"
                {BaseQuery}
                WHERE reservations.status_id = @Status
                ORDER BY reservations.reserved_date DESC";
        var result = await QueryReservationsAsync(sql, new { Status = status });
        return result.Distinct().ToList();
    }

    public async Task<bool> HasUserReservedForDateAsync(int userId, DateTime date)
    {
        var sql = $@"
                SELECT COUNT(1) 
                FROM reservations 
                WHERE reserved_by = @UserId 
                AND reserved_date = @Date
                AND status_id != {(int)ReservationsStatusConstant.Canceled}";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, Date = date.Date });

        return count > 0;
    }
    public async Task<Reservations?> GetUserReservationByDateAsync(int userId, DateTime reservedDate)
    {
        var sql = $@"
                {BaseQuery}
                WHERE reservations.reserved_by = @UserId
                AND reservations.reserved_date = @ReservedDate
                ORDER BY reservations.reserved_date DESC";
        var result = await QueryReservationsAsync(sql, new { UserId = userId, ReservedDate = reservedDate.Date });
        return result.FirstOrDefault();

    }
    public async Task<ReservationGuests?> GetReservationGuestsByidAsync(int reservationGuestsId)
    {
        var sql = $@"SELECT 
            {ReservationGuestsQueries.AllColumns}
            FROM reservation_guests WHERE id = @Id";
        return await _connection.QuerySingleOrDefaultAsync<ReservationGuests>(sql, new { Id = reservationGuestsId });
    }
}
