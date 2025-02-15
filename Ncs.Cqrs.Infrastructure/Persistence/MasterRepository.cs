using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using System.Data;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class MasterRepository : IMasterRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public MasterRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _connection = dbConnectionFactory.CreateConnection();
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }
        public async Task<IEnumerable<Roles>> GetRolesAsync()
        {
            return await _connection.GetListAsync<Roles>();
        }

        public async Task<IEnumerable<Companies>> GetCompaniesAsync()
        {
            return await _connection.GetListAsync<Companies>();
        }
        public async Task<IEnumerable<ReservationsStatus>> GetReservationsStatusAsync()
        {
            return await _connection.GetListAsync<ReservationsStatus>();
        }
        public async Task<IEnumerable<PersonalIdType>> GetPersonalIdTypesAsync()
        {
            return await _connection.GetListAsync<PersonalIdType>();
        }
        public async Task<IEnumerable<Holidays>> GetHolidaysAsync()
        {
            return await _connection.GetListAsync<Holidays>();
        }
        public async Task<IEnumerable<Vendors>> GetVendorsAsync()
        {
            var sql = $@"
                SELECT {VendorQueries.AllColumns}
                FROM vendors
                WHERE vendors.is_deleted = @IsDeleted";
            return await _connection.QueryAsync<Vendors>(sql, new { IsDeleted = false });
        }
        public async Task<Vendors?> GetVendorByIdAsync(int vendorId)
        {
            var sql = $@"
                SELECT {VendorQueries.AllColumns}
                FROM vendors
                WHERE id = @VendorId";

            return await _connection.QueryFirstOrDefaultAsync<Vendors>(sql, new { VendorId = vendorId });
        }


        public async Task<bool> AddVendorAsync(Vendors vendor)
        {
            var affectedRows = await _connection.InsertAsync(vendor);
            return affectedRows > 0;
        }
        public async Task<bool> UpdateVendorAsync(Vendors vendor)
        {
            var affectedRows = await _connection.UpdateAsync(vendor);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteVendorAsync(int vendorId)
        {
            var affectedRows = await _connection.DeleteAsync<Vendors>(vendorId);
            return affectedRows > 0;
        }
    }
}
