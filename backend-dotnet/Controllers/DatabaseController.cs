using Microsoft.AspNetCore.Mvc;
using DentalSpa.Application.Interfaces;
using System.Data;

namespace DentalSpa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseSelectorService _databaseSelector;
        private readonly IDbConnection _connection;
        private readonly ILogger<DatabaseController> _logger;

        public DatabaseController(
            IDatabaseSelectorService databaseSelector,
            IDbConnection connection,
            ILogger<DatabaseController> logger)
        {
            _databaseSelector = databaseSelector;
            _connection = connection;
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
                    ConnectionString = _connection.ConnectionString?.Substring(0, Math.Min(50, _connection.ConnectionString?.Length ?? 0)) + "..."
                },
                SqlServer = new
                {
                    Available = sqlServerAvailable,
                    IsPrimary = false,
                    ConnectionString = "Not configured"
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
                var clients = new List<object>();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, full_name, email, phone FROM clients LIMIT 10";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new
                            {
                                Id = Convert.ToInt32(reader["id"].ToString()),
                                FullName = Convert.ToString(reader["full_name"]?.ToString()),
                                Email = Convert.ToString(reader["email"]?.ToString()),
                                Phone = Convert.ToString(reader["phone"]?.ToString())
                            });
                        }
                    }
                }
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
                return Ok(new { Database = "SQL Server", Count = 0, Data = new List<object>(), Message = "SQL Server not configured" });
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
                return Ok(new { 
                    Message = "Sync not implemented - using only PostgreSQL", 
                    RecordsSynced = 0,
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
                int postgresCount = 0;
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM clients";
                    postgresCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                var comparison = new
                {
                    PostgreSQL = new { ClientCount = postgresCount, Status = "Primary" },
                    SqlServer = new { ClientCount = 0, Status = "Not configured" },
                    InSync = true,
                    Difference = 0
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
                recommendations.Add("Configure SQL Server para redundância se necessário.");
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