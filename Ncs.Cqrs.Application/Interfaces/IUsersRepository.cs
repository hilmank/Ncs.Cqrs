using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces
{
    public interface IUsersRepository
    {
        void SetTransaction(IDbTransaction transaction);
        Task<Users?> GetUsersByIdAsync(int id);
        Task<Users?> GetUsersByUsernameOrEmailAsync(string usernameOrEmail);
        Task<Users?> GetUsersByRfidTagAsync(string rfidTag);
        Task<IEnumerable<Users>> GetUsersByParamsAsync(
            int? rolesId = null,
            int? companyId = null,
            bool? isActive = null,
            string? name = null
            );
        Task<bool> AddUsersAsync(Users User);
        Task<bool> UpdateUsersAsync(Users User);
        Task<bool> DeleteUsersAsync(int UserId);

        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByRfidTagAsync(string rfidTag);

    }
}
