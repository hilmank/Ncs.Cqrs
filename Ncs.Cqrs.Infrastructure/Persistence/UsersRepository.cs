using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Entities;
using Ncs.Cqrs.Domain.Queries;
using Dapper;
using System.Data;
using System.Text;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public UsersRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _connection = dbConnectionFactory.CreateConnection();
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        private string BaseQuery = $@"
            SELECT 
                {UsersQueries.AllColumns},
                {CompaniesQueries.AllColumns},
                {RolesQueries.AllColumns},
                {UsersQueries.AllColumns.Replace("users.", "users_create.")},
                {UsersQueries.AllColumns.Replace("users.", "users_update.")},
                {PersonalIdTypeQueries.AllColumns}
            FROM users
            LEFT JOIN users_roles ON users.id = users_roles.user_id
            LEFT JOIN roles ON users_roles.role_id = roles.id
            LEFT JOIN companies ON users.company_id = companies.id
            JOIN users AS users_create ON users.created_by = users_create.id
            LEFT JOIN users AS users_update ON users.updated_by = users_update.id
            LEFT JOIN personalid_type ON users.personalid_type_id = personalid_type.id
            ";

        private async Task<IEnumerable<Users>> QueryUsersAsync(string sql, object? parameters = null)
        {
            var userDictionary = new Dictionary<int, Users>();

            return await _connection.QueryAsync<Users, Companies, Roles, Users, Users, PersonalIdType, Users>(
                sql,
                (user, company, role, userCreate, userUpdate, personalIdType) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                    {
                        existingUser = user;
                        existingUser.Roles = new List<Roles>();
                        userDictionary[user.Id] = existingUser;
                    }
                    if (role != null)
                        (existingUser.Roles as List<Roles>)?.Add(role);
                    existingUser.Company = company;
                    existingUser.CreatedByUser = userCreate;
                    existingUser.UpdatedByUser = userUpdate;
                    existingUser.PersonalIdType = personalIdType;
                    return existingUser;
                },
                parameters,
                splitOn: "Id,Id,Id,Id,Id"
            );
        }

        public async Task<Users?> GetUsersByUniqueIdAsync(string identifier)
        {
            var sql = $@"
                {BaseQuery}
                WHERE users.username = @Value
                OR users.email = @Value
                OR users.rfid_tag = @Value";

            var users = await QueryUsersAsync(sql, new { Value = identifier });
            return users.FirstOrDefault();
        }

        public async Task<Users?> GetUsersByIdAsync(int id)
        {
            var sql = $@"
                {BaseQuery}
                WHERE users.id = @Id";

            var users = await QueryUsersAsync(sql, new { Id = id });
            return users.FirstOrDefault();
        }
        public async Task<Users?> GetUsersByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var sql = $@"
                {BaseQuery}
                WHERE users.username = @Value OR users.email = @Value";

            var users = await QueryUsersAsync(sql, new { Value = usernameOrEmail });
            return users.FirstOrDefault();
        }
        public async Task<Users?> GetUsersByRfidTagAsync(string rfidTag)
        {
            var sql = $@"
                {BaseQuery}
                WHERE users.rfid_tag = @Value";
            var users = await QueryUsersAsync(sql, new { Value = rfidTag });
            return users.FirstOrDefault();

        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await QueryUsersAsync(BaseQuery);
        }
        public async Task<IEnumerable<Users>> GetUsersByParamsAsync(
            int? rolesId = null,
            int? companyId = null,
            bool? isActive = null,
            string? name = null)
        {
            var sql = new StringBuilder($"{BaseQuery} WHERE 1=1");
            var parameters = new DynamicParameters();

            if (rolesId.HasValue)
            {
                sql.Append(" AND users_roles.role_id = @RolesId");
                parameters.Add("RolesId", rolesId);
            }
            if (companyId.HasValue)
            {
                sql.Append(" AND users.company_id = @CompanyId");
                parameters.Add("CompanyId", companyId);
            }
            if (isActive.HasValue)
            {
                sql.Append(" AND users.is_active = @IsActive");
                parameters.Add("IsActive", isActive);
            }
            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND (users.firstname ILIKE @Name OR users.lastname ILIKE @Name)");
                parameters.Add("Name", $"%{name}%");
            }
            return await QueryUsersAsync(sql.ToString(), parameters);
        }
        public async Task<Users?> GetUsersByRefreshToken(string refreshToken)
        {
            var sql = $@"
                {BaseQuery}
                WHERE users.refresh_token = @Value";
            var users = await QueryUsersAsync(sql, new { Value = refreshToken });
            return users.FirstOrDefault();
        }
        public async Task<bool> AddUsersAsync(Users user)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Insert user and get the new user ID
                var userId = await _connection.InsertAsync(user, transaction);
                if (userId == null || userId == 0)
                    throw new Exception("User insertion failed.");

                // Insert roles if user has roles
                if (user.Roles != null && user.Roles.Any())
                {
                    var sql = "INSERT INTO users_roles (user_id, role_id) VALUES (@UserId, @RoleId)";
                    foreach (var role in user.Roles)
                    {
                        await _connection.ExecuteAsync(sql, new { UserId = userId, RoleId = role.Id }, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Transaction failed", ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<bool> UpdateUsersAsync(Users user)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                if (!user.IsActive)
                {
                    var insertRfidHistorySql = "INSERT INTO rfid_history (user_id, rfid_tag, assigned_at, unassigned_at) VALUES (@UserId, @RfidTag, @AssignedAt, @UnassignedAt)";
                    await _connection.ExecuteAsync(insertRfidHistorySql, new { UserId = user.Id, RfidTag = user.RfidTag, AssignedAt = user.CreatedAt, UnAssignedAt = DateTime.Now }, transaction);
                    user.RfidTag = null;
                }

                // Update user details
                var affectedRows = await _connection.UpdateAsync(user, transaction);
                if (affectedRows == 0)
                    throw new Exception("User update failed or user not found.");


                // Insert updated roles
                if (user.Roles != null && user.Roles.Any())
                {
                    // Remove existing roles (to prevent duplicates)
                    var deleteRolesSql = "DELETE FROM users_roles WHERE user_id = @UserId";
                    await _connection.ExecuteAsync(deleteRolesSql, new { UserId = user.Id }, transaction);

                    var insertRolesSql = "INSERT INTO users_roles (user_id, role_id) VALUES (@UserId, @RoleId)";
                    foreach (var role in user.Roles)
                    {
                        await _connection.ExecuteAsync(insertRolesSql, new { UserId = user.Id, RoleId = role.Id }, transaction);
                    }
                }
                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Transaction failed", ex);
            }
            finally
            {
                _connection.Close();
            }
        }


        public async Task<bool> DeleteUsersAsync(int userId)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Delete user roles first to maintain referential integrity
                var deleteRolesSql = "DELETE FROM users_roles WHERE user_id = @UserId";
                await _connection.ExecuteAsync(deleteRolesSql, new { UserId = userId }, transaction);

                // delete user
                var affectedRows = await _connection.DeleteAsync<Users>(new Users { Id = userId }, transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Transaction failed", ex);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE username = @Username";
            return await _connection.ExecuteScalarAsync<bool>(query, new { Username = username });
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE email = @Email";
            return await _connection.ExecuteScalarAsync<bool>(query, new { Email = email });
        }

        public async Task<bool> ExistsByRfidTagAsync(string rfidTag)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE rfid_tag = @RfidTag";
            return await _connection.ExecuteScalarAsync<bool>(query, new { RfidTag = rfidTag });
        }
    }
}
