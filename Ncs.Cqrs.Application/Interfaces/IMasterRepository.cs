using Ncs.Cqrs.Domain.Entities;
using System.Data;

namespace Ncs.Cqrs.Application.Interfaces
{
    public interface IMasterRepository
    {
        void SetTransaction(IDbTransaction transaction);
        Task<IEnumerable<Roles>> GetRolesAsync();
        Task<IEnumerable<Companies>> GetCompaniesAsync();
        Task<IEnumerable<ReservationsStatus>> GetReservationsStatusAsync();
        Task<IEnumerable<PersonalIdType>> GetPersonalIdTypesAsync();
        Task<IEnumerable<Holidays>> GetHolidaysAsync();
        Task<IEnumerable<Vendors>> GetVendorsAsync();
        Task<Vendors?> GetVendorByIdAsync(int vendorId);
        Task<bool> AddVendorAsync(Vendors vendor);
        Task<bool> UpdateVendorAsync(Vendors vendor);
        Task<bool> DeleteVendorAsync(int vendorId);
    }
}
