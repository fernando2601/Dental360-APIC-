using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IFinancialRepository
    {
        // Transações básicas
        Task<IEnumerable<FinancialTransaction>> GetAllTransactionsAsync();
        Task<FinancialTransaction?> GetTransactionByIdAsync(int id);
        Task<FinancialTransaction> CreateTransactionAsync(CreateFinancialTransactionDto transaction);
        Task<FinancialTransaction?> UpdateTransactionAsync(int id, CreateFinancialTransactionDto transaction);
        Task<bool> DeleteTransactionAsync(int id);

        // Dashboard Financeiro
        Task<FinancialSummary> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate);

        // Fluxo de Caixa
        Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period);
        Task<object> GetCashFlowProjectionsAsync(int months);

        // Análises de Despesas
        Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<CategorySummary>> GetExpenseCategoriesAsync();

        // Relatórios e Análises Avançadas
        Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period);
        Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType);

        // Filtros e buscas
        Task<object> GetTransactionsWithFiltersAsync(
            DateTime? startDate, DateTime? endDate, string? type, string? category, 
            int page, int limit);
    }
}