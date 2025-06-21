using DentalSpa.Application.Interfaces;
using System.Data;

namespace DentalSpa.Application.Services
{
    public class DatabaseSelectorService : IDatabaseSelectorService
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<DatabaseSelectorService> _logger;

        public DatabaseSelectorService(
            IDbConnection connection,
            ILogger<DatabaseSelectorService> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public string GetPrimaryDatabase()
        {
            return "PostgreSQL";
        }

        public string GetSecondaryDatabase()
        {
            return "SQL Server";
        }

        public bool IsPostgreSqlAvailable()
        {
            try
            {
                _connection.Open();
                _connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"PostgreSQL connection failed: {ex.Message}");
                return false;
            }
        }

        public bool IsSqlServerAvailable()
        {
            try
            {
                return false; // SQL Server not configured
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"SQL Server connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync(string connectionType)
        {
            try
            {
                switch (connectionType.ToLower())
                {
                    case "postgresql":
                    case "postgres":
                        _connection.Open();
                        _connection.Close();
                        return await Task.FromResult(true);
                    
                    case "sqlserver":
                    case "sql":
                        return await Task.FromResult(false); // SQL Server not configured
                    
                    default:
                        return await Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database connection test failed for {connectionType}: {ex.Message}");
                return await Task.FromResult(false);
            }
        }
    }
} 