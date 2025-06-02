using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IFinancialService
    {
        // Transações básicas
        Task<IEnumerable<FinancialTransaction>> GetAllTransactionsAsync();
        Task<FinancialTransaction?> GetTransactionByIdAsync(int id);
        Task<FinancialTransaction> CreateTransactionAsync(CreateFinancialTransactionDto transactionDto);
        Task<FinancialTransaction?> UpdateTransactionAsync(int id, CreateFinancialTransactionDto transactionDto);
        Task<bool> DeleteTransactionAsync(int id);

        // Dashboard Financeiro
        Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate);

        // Fluxo de Caixa
        Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period);
        Task<object> GetCashFlowProjectionsAsync(int months);

        // Transações com filtros
        Task<object> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, string? type, string? category, int page, int limit);

        // Análises de Despesas
        Task<object> GetExpensesAsync(DateTime? startDate, DateTime? endDate, string? category);
        Task<IEnumerable<CategorySummary>> GetExpenseCategoriesAsync();
        Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate);

        // Projeções e Análises Avançadas
        Task<object> GetFinancialProjectionsAsync(int months, string type);
        Task<object> CreateProjectionAsync(object projectionData);
        Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType);
        Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period);

        // Relatórios
        Task<FinancialSummary> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GenerateFinancialReportAsync(string format, DateTime? startDate, DateTime? endDate, string reportType);
    }
}