using Microsoft.EntityFrameworkCore;
using DentalSpa.Application.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Application.Services
{
    public class DatabaseSelectorService : IDatabaseSelectorService
    {
        private readonly DentalSpaDbContext _postgresContext;
        private readonly SqlServerDbContext _sqlServerContext;
        private readonly ILogger<DatabaseSelectorService> _logger;

        public DatabaseSelectorService(
            DentalSpaDbContext postgresContext,
            SqlServerDbContext sqlServerContext,
            ILogger<DatabaseSelectorService> logger)
        {
            _postgresContext = postgresContext;
            _sqlServerContext = sqlServerContext;
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
                return _postgresContext.Database.CanConnect();
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
                return _sqlServerContext.Database.CanConnect();
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
                        return await _postgresContext.Database.CanConnectAsync();
                    
                    case "sqlserver":
                    case "sql":
                        return await _sqlServerContext.Database.CanConnectAsync();
                    
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database connection test failed for {connectionType}: {ex.Message}");
                return false;
            }
        }
    }
}