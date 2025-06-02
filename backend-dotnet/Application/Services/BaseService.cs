using Npgsql;
using Dapper;

namespace DentalSpa.API.Services
{
    public abstract class BaseService
    {
        protected readonly string _connectionString;

        public BaseService()
        {
            _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                              "Host=localhost;Database=dental_spa;Username=postgres;Password=postgres;";
        }

        protected async Task<NpgsqlConnection> GetConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
        {
            using var connection = await GetConnectionAsync();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = await GetConnectionAsync();
            return await connection.QueryAsync<T>(sql, param);
        }

        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            using var connection = await GetConnectionAsync();
            return await connection.ExecuteAsync(sql, param);
        }
    }
}