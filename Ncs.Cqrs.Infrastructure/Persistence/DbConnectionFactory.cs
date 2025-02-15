using Ncs.Cqrs.Application.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Ncs.Cqrs.Infrastructure.Persistence
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Database connection string is missing.");
            //SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
        }
        public IDbConnection CreateConnectionSQLServer() => new SqlConnection(_connectionString);
        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
