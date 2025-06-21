using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly IDbConnection _connection;

        public FinancialRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<FinancialTransaction>> GetAllAsync()
        {
            var transactions = new List<FinancialTransaction>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, description, amount, type, category, date, created_at, updated_at FROM financial_transactions WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new FinancialTransaction
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount")),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? null : reader.GetString(reader.GetOrdinal("type")),
                            Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                            Date = reader.IsDBNull(reader.GetOrdinal("date")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("date")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(transactions);
        }

        public async Task<FinancialTransaction?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, description, amount, type, category, date, created_at, updated_at FROM financial_transactions WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new FinancialTransaction
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount")),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? null : reader.GetString(reader.GetOrdinal("type")),
                            Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                            Date = reader.IsDBNull(reader.GetOrdinal("date")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("date")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<FinancialTransaction?>(null);
        }

        public async Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO financial_transactions (description, amount, type, category, date, created_at, updated_at, is_active) 
                                   VALUES (@Description, @Amount, @Type, @Category, @Date, @CreatedAt, @UpdatedAt, 1); 
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@Description", transaction.Description));
                cmd.Parameters.Add(CreateParameter("@Amount", transaction.Amount));
                cmd.Parameters.Add(CreateParameter("@Type", transaction.Type));
                cmd.Parameters.Add(CreateParameter("@Category", transaction.Category));
                cmd.Parameters.Add(CreateParameter("@Date", transaction.Date));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                transaction.Id = id;
                return await Task.FromResult(transaction);
            }
        }

        public async Task<FinancialTransaction?> UpdateAsync(int id, FinancialTransaction transaction)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE financial_transactions SET description = @Description, amount = @Amount, 
                                   type = @Type, category = @Category, date = @Date, updated_at = @UpdatedAt 
                                   WHERE id = @Id AND is_active = 1";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Description", transaction.Description));
                cmd.Parameters.Add(CreateParameter("@Amount", transaction.Amount));
                cmd.Parameters.Add(CreateParameter("@Type", transaction.Type));
                cmd.Parameters.Add(CreateParameter("@Category", transaction.Category));
                cmd.Parameters.Add(CreateParameter("@Date", transaction.Date));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? transaction : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE financial_transactions SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<FinancialTransaction>> SearchAsync(string searchTerm)
        {
            var transactions = new List<FinancialTransaction>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, description, amount, type, category, date, created_at, updated_at FROM financial_transactions WHERE is_active = 1 AND description LIKE @SearchTerm";
                var param = cmd.CreateParameter();
                param.ParameterName = "@SearchTerm";
                param.Value = $"%{searchTerm}%";
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new FinancialTransaction
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Amount = reader.IsDBNull(reader.GetOrdinal("amount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("amount")),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? null : reader.GetString(reader.GetOrdinal("type")),
                            Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                            Date = reader.IsDBNull(reader.GetOrdinal("date")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("date")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(transactions);
        }

        // Analytics methods - returning mock data for now
        public async Task<object> GetFinancialDashboardAsync()
        {
            return await Task.FromResult(new { totalRevenue = 0, totalExpenses = 0, netIncome = 0 });
        }

        public async Task<object> GetCashFlowAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetCashFlowProjectionsAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetTransactionsWithFiltersAsync(string? searchTerm, DateTime? startDate, DateTime? endDate, string? category)
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetExpenseAnalysisAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetExpenseCategoriesAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetAdvancedAnalysisAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetProfitabilityAnalysisAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetFinancialTrendsAsync()
        {
            return await Task.FromResult(new { });
        }

        public async Task<object> GetFinancialSummaryAsync()
        {
            return await Task.FromResult(new { });
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
} 