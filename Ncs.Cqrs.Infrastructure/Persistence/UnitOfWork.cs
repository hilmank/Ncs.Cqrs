using System.Data;
using Ncs.Cqrs.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;
        public IUsersRepository Users { get; }
        public IMasterRepository Masters { get; }
        public IMenuRepository Menus { get; }
        public IMenuSchedulesRepository MenuSchedules { get; }
        public IReservationsRepository Reservations { get; }
        public IOrdersRepository Orders { get; }
        public UnitOfWork(
            IDbConnectionFactory dbConnectionFactory,
            IUsersRepository usersRepository,
            IMasterRepository mastersRepository,
            IMenuRepository menuRepository,
            IMenuSchedulesRepository menuScheduleRepository,
            IReservationsRepository reservationsRepository,
            IOrdersRepository orders)
        {
            _connection = dbConnectionFactory.CreateConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Users = usersRepository;
            Masters = mastersRepository;
            Menus = menuRepository;
            MenuSchedules = menuScheduleRepository;
            Reservations = reservationsRepository;
            Orders = orders;

            // Assign transaction to repositories
            Users.SetTransaction(_transaction);
            Masters.SetTransaction(_transaction);
            Menus.SetTransaction(_transaction);
            MenuSchedules.SetTransaction(_transaction);
            Reservations.SetTransaction(_transaction);
            Orders.SetTransaction(_transaction);
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction = _connection.BeginTransaction(); // Start a new transaction after commit
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction = _connection.BeginTransaction(); // Start a new transaction after rollback
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
