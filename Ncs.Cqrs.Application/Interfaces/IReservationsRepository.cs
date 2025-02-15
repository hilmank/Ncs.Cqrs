using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces;

public interface IReservationsRepository
{
    void SetTransaction(IDbTransaction transaction);
    Task<bool> CreateReservationAsync(Reservations reservation, List<ReservationGuests> guests);
    Task<bool> UpdateReservationAsync(Reservations reservation, List<ReservationGuests> guests);
    Task<bool> DeleteReservationAsync(int reservationId);
    Task<Reservations?> GetReservationByIdAsync(int reservationId);
    Task<List<Reservations>> GetAllReservationsAsync();
    Task<List<Reservations>> GetAllReservationsByDateAsync(DateTime startDate, DateTime endDate);
    Task<List<Reservations>> GetAllReservationsByStatusAsync(int status);
    Task<bool> HasUserReservedForDateAsync(int userId, DateTime date);
    Task<Reservations?> GetUserReservationByDateAsync(int userId, DateTime reservedDate);
    Task<ReservationGuests?> GetReservationGuestsByidAsync(int reservationGuestsId);
}
