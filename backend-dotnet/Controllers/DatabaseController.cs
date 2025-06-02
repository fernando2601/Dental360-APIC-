using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalSpa.Application.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly DentalSpaDbContext _postgresContext;
        private readonly SqlServerDbContext _sqlServerContext;
        private readonly ILogger<DatabaseController> _logger;

        public DatabaseController(
            IDatabaseSelectorService databaseSelector,
            DentalSpaDbContext postgresContext,
            SqlServerDbContext sqlServerContext,
            ILogger<DatabaseController> logger)
        {
            _databaseSelector = databaseSelector;
            _postgresContext = postgresContext;
            _sqlServerContext = sqlServerContext;
            _logger = logger;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetDatabaseStatus()
        {
            var postgresAvailable = await _databaseSelector.TestConnectionAsync("postgresql");
            var sqlServerAvailable = await _databaseSelector.TestConnectionAsync("sqlserver");

            var status = new
            {
                PostgreSQL = new
                {
                    Available = postgresAvailable,
                    IsPrimary = true,
                    ConnectionString = _postgresContext.Database.GetConnectionString()?.Substring(0, 50) + "..."
                },
                SqlServer = new
                {
                    Available = sqlServerAvailable,
                    IsPrimary = false,
                    ConnectionString = _sqlServerContext.Database.GetConnectionString()?.Substring(0, 50) + "..."
                },
                Recommendations = GetRecommendations(postgresAvailable, sqlServerAvailable)
            };

            return Ok(status);
        }

        [HttpGet("clients/postgresql")]
        public async Task<IActionResult> GetClientsFromPostgreSQL()
        {
            try
            {
                var clients = await _postgresContext.Clients.Take(10).ToListAsync();
                return Ok(new { Database = "PostgreSQL", Count = clients.Count, Data = clients });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error accessing PostgreSQL: {ex.Message}");
                return BadRequest(new { Error = "PostgreSQL not available", Message = ex.Message });
            }
        }

        [HttpGet("clients/sqlserver")]
        public async Task<IActionResult> GetClientsFromSqlServer()
        {
            try
            {
                var clients = await _sqlServerContext.Clients.Take(10).ToListAsync();
                return Ok(new { Database = "SQL Server", Count = clients.Count, Data = clients });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error accessing SQL Server: {ex.Message}");
                return BadRequest(new { Error = "SQL Server not available", Message = ex.Message });
            }
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncDatabases()
        {
            try
            {
                // Exemplo de sincronização: copiar clientes do PostgreSQL para SQL Server
                var postgresClients = await _postgresContext.Clients.ToListAsync();
                
                // Limpar tabela SQL Server
                _sqlServerContext.Clients.RemoveRange(_sqlServerContext.Clients);
                
                // Adicionar clientes do PostgreSQL
                await _sqlServerContext.Clients.AddRangeAsync(postgresClients);
                var synced = await _sqlServerContext.SaveChangesAsync();

                return Ok(new { 
                    Message = "Databases synchronized successfully", 
                    RecordsSynced = synced,
                    From = "PostgreSQL",
                    To = "SQL Server"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error syncing databases: {ex.Message}");
                return BadRequest(new { Error = "Sync failed", Message = ex.Message });
            }
        }

        [HttpGet("compare")]
        public async Task<IActionResult> CompareDatabases()
        {
            try
            {
                var postgresCount = await _postgresContext.Clients.CountAsync();
                var sqlServerCount = await _sqlServerContext.Clients.CountAsync();

                var comparison = new
                {
                    PostgreSQL = new { ClientCount = postgresCount, Status = "Primary" },
                    SqlServer = new { ClientCount = sqlServerCount, Status = "Secondary" },
                    InSync = postgresCount == sqlServerCount,
                    Difference = Math.Abs(postgresCount - sqlServerCount)
                };

                return Ok(comparison);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Comparison failed", Message = ex.Message });
            }
        }

        private static List<string> GetRecommendations(bool postgresAvailable, bool sqlServerAvailable)
        {
            var recommendations = new List<string>();

            if (!postgresAvailable && !sqlServerAvailable)
            {
                recommendations.Add("Nenhum banco de dados está disponível. Verifique as conexões.");
            }
            else if (postgresAvailable && !sqlServerAvailable)
            {
                recommendations.Add("PostgreSQL está funcionando como banco principal.");
                recommendations.Add("Configure SQL Server para redundância.");
            }
            else if (!postgresAvailable && sqlServerAvailable)
            {
                recommendations.Add("PostgreSQL indisponível. SQL Server está funcionando como fallback.");
                recommendations.Add("Restaure a conexão PostgreSQL o mais rápido possível.");
            }
            else
            {
                recommendations.Add("Ambos os bancos estão funcionando perfeitamente.");
                recommendations.Add("Use PostgreSQL como principal e SQL Server para backup/relatórios.");
            }

            return recommendations;
        }
    }
}