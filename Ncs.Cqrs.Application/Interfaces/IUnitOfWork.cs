namespace Ncs.Cqrs.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository Users { get; }
        IMasterRepository Masters { get; }
        IMenuRepository Menus { get; }
        IMenuSchedulesRepository MenuSchedules { get; }
        IReservationsRepository Reservations { get; }
        IOrdersRepository Orders { get; }
        void Commit();  // Commit transaction
        void Rollback(); // Rollback transaction
    }

}
