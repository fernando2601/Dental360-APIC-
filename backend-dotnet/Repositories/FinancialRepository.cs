using Dapper;
using Npgsql;
using ClinicApi.Models;
using System.Text;

namespace ClinicApi.Repositories
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public FinancialRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<FinancialTransaction>> GetAllTransactionsAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    type as Type,
                    category as Category,
                    amount as Amount,
                    description as Description,
                    transaction_date as TransactionDate,
                    payment_method as PaymentMethod,
                    client_id as ClientId,
                    appointment_id as AppointmentId,
                    reference_number as ReferenceNumber,
                    status as Status,
                    created_at as CreatedAt
                FROM financial_transactions 
                ORDER BY transaction_date DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<FinancialTransaction>(sql);
        }

        public async Task<FinancialTransaction?> GetTransactionByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    type as Type,
                    category as Category,
                    amount as Amount,
                    description as Description,
                    transaction_date as TransactionDate,
                    payment_method as PaymentMethod,
                    client_id as ClientId,
                    appointment_id as AppointmentId,
                    reference_number as ReferenceNumber,
                    status as Status,
                    created_at as CreatedAt
                FROM financial_transactions 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<FinancialTransaction>(sql, new { Id = id });
        }

        public async Task<FinancialTransaction> CreateTransactionAsync(CreateFinancialTransactionDto transactionDto)
        {
            const string sql = @"
                INSERT INTO financial_transactions 
                (type, category, amount, description, transaction_date, payment_method, 
                 client_id, appointment_id, reference_number, status, created_at)
                VALUES 
                (@Type, @Category, @Amount, @Description, @TransactionDate, @PaymentMethod, 
                 @ClientId, @AppointmentId, @ReferenceNumber, @Status, @CreatedAt)
                RETURNING 
                    id as Id,
                    type as Type,
                    category as Category,
                    amount as Amount,
                    description as Description,
                    transaction_date as TransactionDate,
                    payment_method as PaymentMethod,
                    client_id as ClientId,
                    appointment_id as AppointmentId,
                    reference_number as ReferenceNumber,
                    status as Status,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<FinancialTransaction>(sql, new
            {
                transactionDto.Type,
                transactionDto.Category,
                transactionDto.Amount,
                transactionDto.Description,
                transactionDto.TransactionDate,
                transactionDto.PaymentMethod,
                transactionDto.ClientId,
                transactionDto.AppointmentId,
                transactionDto.ReferenceNumber,
                transactionDto.Status,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<FinancialTransaction?> UpdateTransactionAsync(int id, CreateFinancialTransactionDto transactionDto)
        {
            const string sql = @"
                UPDATE financial_transactions 
                SET 
                    type = @Type,
                    category = @Category,
                    amount = @Amount,
                    description = @Description,
                    transaction_date = @TransactionDate,
                    payment_method = @PaymentMethod,
                    client_id = @ClientId,
                    appointment_id = @AppointmentId,
                    reference_number = @ReferenceNumber,
                    status = @Status
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    type as Type,
                    category as Category,
                    amount as Amount,
                    description as Description,
                    transaction_date as TransactionDate,
                    payment_method as PaymentMethod,
                    client_id as ClientId,
                    appointment_id as AppointmentId,
                    reference_number as ReferenceNumber,
                    status as Status,
                    created_at as CreatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<FinancialTransaction>(sql, new
            {
                Id = id,
                transactionDto.Type,
                transactionDto.Category,
                transactionDto.Amount,
                transactionDto.Description,
                transactionDto.TransactionDate,
                transactionDto.PaymentMethod,
                transactionDto.ClientId,
                transactionDto.AppointmentId,
                transactionDto.ReferenceNumber,
                transactionDto.Status
            });
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            const string sql = "DELETE FROM financial_transactions WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<FinancialSummary> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                SELECT 
                    COALESCE(SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END), 0) as TotalIncome,
                    COALESCE(SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END), 0) as TotalExpenses,
                    COUNT(*) as TotalTransactions
                FROM financial_transactions 
                WHERE status = 'completed' {whereClause}";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            
            var result = await connection.QuerySingleAsync(sql, parameters);
            
            var summary = new FinancialSummary
            {
                TotalIncome = result.TotalIncome,
                TotalExpenses = result.TotalExpenses,
                NetProfit = result.TotalIncome - result.TotalExpenses,
                TotalTransactions = result.TotalTransactions
            };

            // Buscar breakdown por categoria
            summary.IncomeByCategory = await GetCategoryBreakdown("income", startDate, endDate);
            summary.ExpensesByCategory = await GetCategoryBreakdown("expense", startDate, endDate);

            return summary;
        }

        public async Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate)
        {
            var summary = await GetFinancialSummaryAsync(startDate, endDate);
            var cashFlow = await GetCashFlowAsync(startDate, endDate, "daily");
            
            // Métricas adicionais para dashboard
            var whereClause = BuildDateFilter(startDate, endDate);
            var sql = $@"
                SELECT 
                    COUNT(DISTINCT client_id) as UniqueClients,
                    AVG(amount) as AverageTransactionValue,
                    COUNT(CASE WHEN payment_method = 'dinheiro' THEN 1 END) as CashTransactions,
                    COUNT(CASE WHEN payment_method = 'cartao' THEN 1 END) as CardTransactions,
                    COUNT(CASE WHEN payment_method = 'pix' THEN 1 END) as PixTransactions
                FROM financial_transactions 
                WHERE status = 'completed' AND type = 'income' {whereClause}";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            var metrics = await connection.QuerySingleAsync(sql, parameters);

            return new
            {
                summary,
                cashFlow,
                metrics = new
                {
                    uniqueClients = metrics.UniqueClients,
                    averageTransactionValue = metrics.AverageTransactionValue,
                    paymentMethods = new
                    {
                        cash = metrics.CashTransactions,
                        card = metrics.CardTransactions,
                        pix = metrics.PixTransactions
                    }
                }
            };
        }

        public async Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var dateGrouping = period.ToLower() switch
            {
                "daily" => "DATE(transaction_date)",
                "weekly" => "DATE_TRUNC('week', transaction_date)",
                "monthly" => "DATE_TRUNC('month', transaction_date)",
                "yearly" => "DATE_TRUNC('year', transaction_date)",
                _ => "DATE(transaction_date)"
            };

            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                SELECT 
                    {dateGrouping} as Period,
                    COALESCE(SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END), 0) as Income,
                    COALESCE(SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END), 0) as Expenses,
                    COALESCE(SUM(CASE WHEN type = 'income' THEN amount ELSE -amount END), 0) as NetFlow
                FROM financial_transactions 
                WHERE status = 'completed' {whereClause}
                GROUP BY {dateGrouping}
                ORDER BY Period";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            var cashFlow = await connection.QueryAsync(sql, parameters);

            // Calcular saldo acumulado
            decimal cumulativeBalance = 0;
            var flowWithBalance = cashFlow.Select(item => new
            {
                period = item.Period,
                income = item.Income,
                expenses = item.Expenses,
                netFlow = item.NetFlow,
                cumulativeBalance = cumulativeBalance += item.NetFlow
            }).ToList();

            return new
            {
                period,
                data = flowWithBalance,
                summary = new
                {
                    totalIncome = flowWithBalance.Sum(x => x.income),
                    totalExpenses = flowWithBalance.Sum(x => x.expenses),
                    totalNetFlow = flowWithBalance.Sum(x => x.netFlow),
                    finalBalance = cumulativeBalance
                }
            };
        }

        public async Task<object> GetCashFlowProjectionsAsync(int months)
        {
            // Análise histórica para projeção
            var sql = @"
                SELECT 
                    DATE_TRUNC('month', transaction_date) as Month,
                    AVG(CASE WHEN type = 'income' THEN amount ELSE 0 END) as AvgIncome,
                    AVG(CASE WHEN type = 'expense' THEN amount ELSE 0 END) as AvgExpenses
                FROM financial_transactions 
                WHERE status = 'completed' 
                    AND transaction_date >= NOW() - INTERVAL '12 months'
                GROUP BY DATE_TRUNC('month', transaction_date)
                ORDER BY Month";

            using var connection = new NpgsqlConnection(_connectionString);
            var historical = await connection.QueryAsync(sql);

            var avgMonthlyIncome = historical.Any() ? historical.Average(x => x.AvgIncome) : 0;
            var avgMonthlyExpenses = historical.Any() ? historical.Average(x => x.AvgExpenses) : 0;

            // Gerar projeções
            var projections = new List<object>();
            var currentDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day); // Primeiro dia do próximo mês

            for (int i = 0; i < months; i++)
            {
                var projectedMonth = currentDate.AddMonths(i);
                projections.Add(new
                {
                    month = projectedMonth,
                    projectedIncome = avgMonthlyIncome * (1 + (i * 0.02m)), // Crescimento de 2% ao mês
                    projectedExpenses = avgMonthlyExpenses * (1 + (i * 0.01m)), // Crescimento de 1% ao mês
                    projectedNetFlow = (avgMonthlyIncome * (1 + (i * 0.02m))) - (avgMonthlyExpenses * (1 + (i * 0.01m)))
                });
            }

            return new
            {
                historicalData = historical,
                projections,
                assumptions = new
                {
                    monthlyIncomeGrowth = 0.02m,
                    monthlyExpenseGrowth = 0.01m,
                    basedOnMonths = 12
                }
            };
        }

        public async Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                SELECT 
                    category,
                    COUNT(*) as TransactionCount,
                    SUM(amount) as TotalAmount,
                    AVG(amount) as AverageAmount,
                    MIN(amount) as MinAmount,
                    MAX(amount) as MaxAmount
                FROM financial_transactions 
                WHERE type = 'expense' AND status = 'completed' {whereClause}
                GROUP BY category
                ORDER BY TotalAmount DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            var analysis = await connection.QueryAsync(sql, parameters);

            var totalExpenses = analysis.Sum(x => x.TotalAmount);

            return new
            {
                categoryAnalysis = analysis.Select(item => new
                {
                    category = item.category,
                    transactionCount = item.TransactionCount,
                    totalAmount = item.TotalAmount,
                    averageAmount = item.AverageAmount,
                    minAmount = item.MinAmount,
                    maxAmount = item.MaxAmount,
                    percentage = totalExpenses > 0 ? (decimal)item.TotalAmount / totalExpenses * 100 : 0
                }),
                summary = new
                {
                    totalExpenses,
                    categoryCount = analysis.Count(),
                    averagePerCategory = analysis.Any() ? analysis.Average(x => x.TotalAmount) : 0
                }
            };
        }

        public async Task<IEnumerable<CategorySummary>> GetExpenseCategoriesAsync()
        {
            const string sql = @"
                SELECT 
                    category as Category,
                    SUM(amount) as Amount,
                    COUNT(*) as Count
                FROM financial_transactions 
                WHERE type = 'expense' AND status = 'completed'
                    AND transaction_date >= NOW() - INTERVAL '30 days'
                GROUP BY category
                ORDER BY Amount DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var categories = await connection.QueryAsync(sql);

            var total = categories.Sum(x => x.Amount);

            return categories.Select(item => new CategorySummary
            {
                Category = item.Category,
                Amount = item.Amount,
                Count = item.Count,
                Percentage = total > 0 ? (decimal)item.Amount / total * 100 : 0
            });
        }

        public async Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                WITH monthly_data AS (
                    SELECT 
                        DATE_TRUNC('month', transaction_date) as Month,
                        SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END) as Income,
                        SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END) as Expenses
                    FROM financial_transactions 
                    WHERE status = 'completed' {whereClause}
                    GROUP BY DATE_TRUNC('month', transaction_date)
                )
                SELECT 
                    Month,
                    Income,
                    Expenses,
                    (Income - Expenses) as Profit,
                    CASE WHEN Income > 0 THEN (Income - Expenses) / Income * 100 ELSE 0 END as ProfitMargin
                FROM monthly_data
                ORDER BY Month";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            var profitability = await connection.QueryAsync(sql, parameters);

            return new
            {
                monthlyProfitability = profitability,
                summary = new
                {
                    totalProfit = profitability.Sum(x => x.Profit),
                    averageProfitMargin = profitability.Any() ? profitability.Average(x => x.ProfitMargin) : 0,
                    bestMonth = profitability.OrderByDescending(x => x.Profit).FirstOrDefault(),
                    worstMonth = profitability.OrderBy(x => x.Profit).FirstOrDefault()
                }
            };
        }

        public async Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var dateGrouping = period.ToLower() switch
            {
                "daily" => "DATE(transaction_date)",
                "weekly" => "DATE_TRUNC('week', transaction_date)",
                "monthly" => "DATE_TRUNC('month', transaction_date)",
                _ => "DATE_TRUNC('month', transaction_date)"
            };

            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                SELECT 
                    {dateGrouping} as Period,
                    SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END) as Income,
                    SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END) as Expenses,
                    COUNT(CASE WHEN type = 'income' THEN 1 END) as IncomeTransactions,
                    COUNT(CASE WHEN type = 'expense' THEN 1 END) as ExpenseTransactions
                FROM financial_transactions 
                WHERE status = 'completed' {whereClause}
                GROUP BY {dateGrouping}
                ORDER BY Period";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            var trends = await connection.QueryAsync(sql, parameters);

            // Calcular tendências (crescimento percentual)
            var trendsWithGrowth = trends.Select((item, index) => 
            {
                var previous = index > 0 ? trends.ElementAt(index - 1) : null;
                return new
                {
                    period = item.Period,
                    income = item.Income,
                    expenses = item.Expenses,
                    incomeTransactions = item.IncomeTransactions,
                    expenseTransactions = item.ExpenseTransactions,
                    incomeGrowth = previous != null && previous.Income > 0 
                        ? ((decimal)item.Income - previous.Income) / previous.Income * 100 
                        : 0,
                    expenseGrowth = previous != null && previous.Expenses > 0 
                        ? ((decimal)item.Expenses - previous.Expenses) / previous.Expenses * 100 
                        : 0
                };
            });

            return new
            {
                period,
                trends = trendsWithGrowth,
                summary = new
                {
                    averageIncomeGrowth = trendsWithGrowth.Where(x => x.incomeGrowth != 0).Any() 
                        ? trendsWithGrowth.Where(x => x.incomeGrowth != 0).Average(x => x.incomeGrowth) 
                        : 0,
                    averageExpenseGrowth = trendsWithGrowth.Where(x => x.expenseGrowth != 0).Any() 
                        ? trendsWithGrowth.Where(x => x.expenseGrowth != 0).Average(x => x.expenseGrowth) 
                        : 0
                }
            };
        }

        public async Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType)
        {
            return analysisType.ToLower() switch
            {
                "complete" => await GetCompleteAnalysisAsync(startDate, endDate),
                "profitability" => await GetProfitabilityAnalysisAsync(startDate, endDate),
                "trends" => await GetFinancialTrendsAsync(startDate, endDate, "monthly"),
                "cashflow" => await GetCashFlowAsync(startDate, endDate, "monthly"),
                _ => await GetCompleteAnalysisAsync(startDate, endDate)
            };
        }

        public async Task<object> GetTransactionsWithFiltersAsync(
            DateTime? startDate, DateTime? endDate, string? type, string? category, 
            int page, int limit)
        {
            var whereConditions = new List<string> { "status = 'completed'" };
            var parameters = new DynamicParameters();

            if (startDate.HasValue)
            {
                whereConditions.Add("transaction_date >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                whereConditions.Add("transaction_date <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }

            if (!string.IsNullOrEmpty(type))
            {
                whereConditions.Add("type = @Type");
                parameters.Add("Type", type);
            }

            if (!string.IsNullOrEmpty(category))
            {
                whereConditions.Add("category = @Category");
                parameters.Add("Category", category);
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var offset = (page - 1) * limit;

            // Count total
            var countSql = $"SELECT COUNT(*) FROM financial_transactions WHERE {whereClause}";
            
            // Get transactions
            var dataSql = $@"
                SELECT 
                    id as Id,
                    type as Type,
                    category as Category,
                    amount as Amount,
                    description as Description,
                    transaction_date as TransactionDate,
                    payment_method as PaymentMethod,
                    client_id as ClientId,
                    appointment_id as AppointmentId,
                    reference_number as ReferenceNumber,
                    status as Status,
                    created_at as CreatedAt
                FROM financial_transactions 
                WHERE {whereClause}
                ORDER BY transaction_date DESC
                LIMIT {limit} OFFSET {offset}";

            using var connection = new NpgsqlConnection(_connectionString);
            
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);
            var transactions = await connection.QueryAsync<FinancialTransaction>(dataSql, parameters);

            return new
            {
                transactions,
                pagination = new
                {
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / limit),
                    totalItems = totalCount,
                    itemsPerPage = limit
                }
            };
        }

        // Métodos auxiliares
        private string BuildDateFilter(DateTime? startDate, DateTime? endDate)
        {
            var conditions = new List<string>();
            
            if (startDate.HasValue)
                conditions.Add("AND transaction_date >= @StartDate");
            
            if (endDate.HasValue)
                conditions.Add("AND transaction_date <= @EndDate");

            return string.Join(" ", conditions);
        }

        private DynamicParameters BuildDateParameters(DateTime? startDate, DateTime? endDate)
        {
            var parameters = new DynamicParameters();
            
            if (startDate.HasValue)
                parameters.Add("StartDate", startDate.Value);
            
            if (endDate.HasValue)
                parameters.Add("EndDate", endDate.Value);

            return parameters;
        }

        private async Task<List<CategorySummary>> GetCategoryBreakdown(string type, DateTime? startDate, DateTime? endDate)
        {
            var whereClause = BuildDateFilter(startDate, endDate);
            
            var sql = $@"
                SELECT 
                    category as Category,
                    SUM(amount) as Amount,
                    COUNT(*) as Count
                FROM financial_transactions 
                WHERE type = @Type AND status = 'completed' {whereClause}
                GROUP BY category
                ORDER BY Amount DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var parameters = BuildDateParameters(startDate, endDate);
            parameters.Add("Type", type);
            
            var breakdown = await connection.QueryAsync(sql, parameters);
            var total = breakdown.Sum(x => x.Amount);

            return breakdown.Select(item => new CategorySummary
            {
                Category = item.Category,
                Amount = item.Amount,
                Count = item.Count,
                Percentage = total > 0 ? (decimal)item.Amount / total * 100 : 0
            }).ToList();
        }

        private async Task<object> GetCompleteAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var summary = await GetFinancialSummaryAsync(startDate, endDate);
            var cashFlow = await GetCashFlowAsync(startDate, endDate, "monthly");
            var trends = await GetFinancialTrendsAsync(startDate, endDate, "monthly");
            var profitability = await GetProfitabilityAnalysisAsync(startDate, endDate);
            var expenseAnalysis = await GetExpenseAnalysisAsync(startDate, endDate);

            return new
            {
                summary,
                cashFlow,
                trends,
                profitability,
                expenseAnalysis,
                generatedAt = DateTime.UtcNow
            };
        }
    }
}