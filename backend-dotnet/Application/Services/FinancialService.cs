using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Text;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IFinancialRepository _financialRepository;

        public FinancialService(IFinancialRepository financialRepository)
        {
            _financialRepository = financialRepository;
        }

        public async Task<IEnumerable<FinancialTransactionResponse>> GetAllAsync()
        {
            var transactions = await _financialRepository.GetAllAsync();
            return transactions.Select(MapToResponse);
        }

        public async Task<FinancialTransactionResponse?> GetByIdAsync(int id)
        {
            var transaction = await _financialRepository.GetByIdAsync(id);
            return transaction == null ? null : MapToResponse(transaction);
        }

        public async Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction)
        {
            ValidateTransaction(transaction);
            if (string.IsNullOrEmpty(transaction.ReferenceNumber))
            {
                transaction.ReferenceNumber = GenerateReferenceNumber(transaction.Type);
            }
            transaction.CreatedAt = DateTime.UtcNow;
            return await _financialRepository.CreateAsync(transaction);
        }

        public async Task<FinancialTransaction?> UpdateAsync(int id, FinancialTransaction transaction)
        {
            ValidateTransaction(transaction);
            return await _financialRepository.UpdateAsync(id, transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _financialRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<FinancialTransactionResponse>> SearchAsync(string searchTerm)
        {
            var transactions = await _financialRepository.SearchAsync(searchTerm);
            return transactions.Select(MapToResponse);
        }

        public async Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            var totalRevenue = filteredTransactions.Where(t => t.Type == "income").Sum(t => t.Amount);
            var totalExpenses = filteredTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
            var profit = totalRevenue - totalExpenses;

            return new
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                Profit = profit,
                Period = new { Start = start, End = end },
                TransactionCount = filteredTransactions.Count()
            };
        }

        public async Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            
            var cashFlow = period.ToLower() switch
            {
                "monthly" => filteredTransactions.GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        NetFlow = g.Where(t => t.Type == "income").Sum(t => t.Amount) - g.Where(t => t.Type == "expense").Sum(t => t.Amount)
                    }).OrderBy(x => x.Period),
                "weekly" => filteredTransactions.GroupBy(t => new { Year = t.Date.Year, Week = GetWeekOfYear(t.Date) })
                    .Select(g => new
                    {
                        Period = $"Week {g.Key.Week}, {g.Key.Year}",
                        Revenue = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        NetFlow = g.Where(t => t.Type == "income").Sum(t => t.Amount) - g.Where(t => t.Type == "expense").Sum(t => t.Amount)
                    }).OrderBy(x => x.Period),
                _ => filteredTransactions.GroupBy(t => t.Date.Date)
                    .Select(g => new
                    {
                        Period = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        NetFlow = g.Where(t => t.Type == "income").Sum(t => t.Amount) - g.Where(t => t.Type == "expense").Sum(t => t.Amount)
                    }).OrderBy(x => x.Period)
            };

            return cashFlow;
        }

        public async Task<object> GetExpensesAsync(DateTime? startDate, DateTime? endDate, string? category)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end && t.Type == "expense");
            
            if (!string.IsNullOrEmpty(category))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Category == category);
            }

            return filteredTransactions.ToList();
        }

        public async Task<object> GetExpenseCategoriesAsync()
        {
            var transactions = await _financialRepository.GetAllAsync();
            var expenseCategories = transactions.Where(t => t.Type == "expense")
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount), Count = g.Count() })
                .OrderByDescending(x => x.Total);

            return expenseCategories;
        }

        public async Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var expenses = transactions.Where(t => t.Date >= start && t.Date <= end && t.Type == "expense");
            
            var analysis = new
            {
                TotalExpenses = expenses.Sum(t => t.Amount),
                AverageExpense = expenses.Any() ? expenses.Average(t => t.Amount) : 0,
                ExpenseByCategory = expenses.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount), Count = g.Count() })
                    .OrderByDescending(x => x.Total),
                MonthlyExpenses = expenses.GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:D2}", Total = g.Sum(t => t.Amount) })
                    .OrderBy(x => x.Month)
            };

            return analysis;
        }

        public async Task<object> GetFinancialProjectionsAsync(int months, string type)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var recentMonths = transactions.GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Take(6);

            var avgMonthlyRevenue = recentMonths.Average(g => g.Where(t => t.Type == "income").Sum(t => t.Amount));
            var avgMonthlyExpenses = recentMonths.Average(g => g.Where(t => t.Type == "expense").Sum(t => t.Amount));

            var projections = Enumerable.Range(1, months).Select((int i) => new
            {
                Month = DateTime.Now.AddMonths(i).ToString("yyyy-MM"),
                ProjectedRevenue = avgMonthlyRevenue,
                ProjectedExpenses = avgMonthlyExpenses,
                ProjectedNetFlow = avgMonthlyRevenue - avgMonthlyExpenses
            });

            return projections;
        }

        public async Task<object> CreateProjectionAsync(object projectionData)
        {
            // Implementação básica - retorna os dados recebidos
            return new { Success = true, Data = projectionData, CreatedAt = DateTime.UtcNow };
        }

        public async Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            var revenue = filteredTransactions.Where(t => t.Type == "income");
            var expenses = filteredTransactions.Where(t => t.Type == "expense");

            var analysis = new
            {
                RevenueGrowth = CalculateGrowthRate(revenue),
                ExpenseGrowth = CalculateGrowthRate(expenses),
                ProfitMargin = revenue.Any() ? ((revenue.Sum(t => t.Amount) - expenses.Sum(t => t.Amount)) / revenue.Sum(t => t.Amount)) * 100 : 0,
                TopRevenueCategories = revenue.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Total)
                    .Take(5),
                TopExpenseCategories = expenses.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Total)
                    .Take(5),
                AnalysisType = analysisType
            };

            return analysis;
        }

        public async Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            var revenue = filteredTransactions.Where(t => t.Type == "income");
            var expenses = filteredTransactions.Where(t => t.Type == "expense");

            var totalRevenue = revenue.Sum(t => t.Amount);
            var totalExpenses = expenses.Sum(t => t.Amount);
            var netProfit = totalRevenue - totalExpenses;

            return new
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ProfitMargin = totalRevenue > 0 ? (netProfit / totalRevenue) * 100 : 0,
                RevenueByCategory = revenue.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount), Percentage = (g.Sum(t => t.Amount) / totalRevenue) * 100 }),
                ExpenseByCategory = expenses.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount), Percentage = totalExpenses > 0 ? (g.Sum(t => t.Amount) / totalExpenses) * 100 : 0 })
            };
        }

        public async Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            
            var trends = period.ToLower() switch
            {
                "monthly" => filteredTransactions.GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        NetFlow = g.Where(t => t.Type == "income").Sum(t => t.Amount) - g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        Trend = CalculateTrend(g)
                    }).OrderBy(x => x.Period),
                _ => filteredTransactions.GroupBy(t => t.Date.Date)
                    .Select(g => new
                    {
                        Period = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        NetFlow = g.Where(t => t.Type == "income").Sum(t => t.Amount) - g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        Trend = CalculateTrend(g)
                    }).OrderBy(x => x.Period)
            };

            return trends;
        }

        public async Task<object> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _financialRepository.GetAllAsync();
            var (start, end) = NormalizeDateRange(startDate, endDate);
            
            var filteredTransactions = transactions.Where(t => t.Date >= start && t.Date <= end);
            var revenue = filteredTransactions.Where(t => t.Type == "income");
            var expenses = filteredTransactions.Where(t => t.Type == "expense");

            return new FinancialSummary
            {
                TotalIncome = revenue.Sum(t => t.Amount),
                TotalExpenses = expenses.Sum(t => t.Amount),
                NetProfit = revenue.Sum(t => t.Amount) - expenses.Sum(t => t.Amount),
                TotalTransactions = filteredTransactions.Count(),
                IncomeByCategory = revenue.GroupBy(t => t.Category)
                    .Select(g => new CategorySummary { Category = g.Key, Amount = g.Sum(t => t.Amount), Count = g.Count() }).ToList(),
                ExpensesByCategory = expenses.GroupBy(t => t.Category)
                    .Select(g => new CategorySummary { Category = g.Key, Amount = g.Sum(t => t.Amount), Count = g.Count() }).ToList()
            };
        }

        public async Task<dynamic> GenerateFinancialReportAsync(string format, DateTime? startDate, DateTime? endDate, string reportType)
        {
            var summary = await GetFinancialSummaryAsync(startDate, endDate);
            
            // Implementação básica - retorna um objeto com dados do relatório
            return new
            {
                Content = Encoding.UTF8.GetBytes("Relatório financeiro gerado"),
                ContentType = format.ToLower() == "pdf" ? "application/pdf" : "application/json",
                FileName = $"financial_report_{DateTime.Now:yyyyMMdd}.{format}",
                Summary = summary
            };
        }

        private double CalculateGrowthRate(IEnumerable<FinancialTransaction> transactions)
        {
            if (!transactions.Any()) return 0;
            
            var monthlyData = transactions.GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => (double)g.Sum(t => t.Amount))
                .ToList();

            if (monthlyData.Count < 2) return 0;

            var current = monthlyData.Last();
            var previous = monthlyData[monthlyData.Count - 2];

            return previous > 0 ? ((current - previous) / previous) * 100 : 0;
        }

        private string CalculateTrend(IEnumerable<FinancialTransaction> transactions)
        {
            if (!transactions.Any()) return "stable";
            
            var amounts = transactions.Select(t => t.Amount).ToList();
            if (amounts.Count < 2) return "stable";

            var trend = amounts.Last() - amounts.First();
            return trend > 0 ? "increasing" : trend < 0 ? "decreasing" : "stable";
        }

        private void ValidateTransaction(FinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrEmpty(transaction.Type))
                throw new ArgumentException("Transaction type is required");

            if (transaction.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero");

            if (string.IsNullOrEmpty(transaction.Description))
                throw new ArgumentException("Transaction description is required");
        }

        private string GenerateReferenceNumber(string type)
        {
            var prefix = type.ToUpper().Substring(0, Math.Min(3, type.Length));
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}{timestamp}{random}";
        }

        private (DateTime start, DateTime end) NormalizeDateRange(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;
            return (start, end);
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private FinancialTransactionResponse MapToResponse(FinancialTransaction t)
        {
            return new FinancialTransactionResponse
            {
                Type = t.Type,
                Category = t.Category,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                PaymentMethod = t.PaymentMethod,
                ClientId = t.ClientId,
                AppointmentId = t.AppointmentId,
                ReferenceNumber = t.ReferenceNumber,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            };
        }
    }
}