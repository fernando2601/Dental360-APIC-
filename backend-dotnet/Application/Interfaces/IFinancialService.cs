using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IFinancialService
    {
        Task<IEnumerable<FinancialTransaction>> GetAllAsync();
        Task<FinancialTransaction?> GetByIdAsync(int id);
        Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction);
        Task<FinancialTransaction?> UpdateAsync(int id, FinancialTransaction transaction);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FinancialTransaction>> SearchAsync(string searchTerm);
        
        // Analytics methods
        Task<object> GetFinancialDashboardAsync();
        Task<object> GetCashFlowAsync();
        Task<object> GetCashFlowProjectionsAsync();
        Task<object> GetTransactionsWithFiltersAsync(string? searchTerm, DateTime? startDate, DateTime? endDate, string? category);
        Task<object> GetExpenseAnalysisAsync();
        Task<object> GetExpenseCategoriesAsync();
        Task<object> GetAdvancedAnalysisAsync();
        Task<object> GetProfitabilityAnalysisAsync();
        Task<object> GetFinancialTrendsAsync();
        Task<object> GetFinancialSummaryAsync();
        Task<object> ExportFinancialReportAsync();
    }
} 