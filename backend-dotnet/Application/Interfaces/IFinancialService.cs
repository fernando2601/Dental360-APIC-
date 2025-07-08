using DentalSpa.Domain.Entities;
using DentalSpa.Application.DTOs;

namespace DentalSpa.Application.Interfaces
{
    public interface IFinancialService
    {
        Task<IEnumerable<FinancialTransactionResponse>> GetAllAsync();
        Task<FinancialTransactionResponse?> GetByIdAsync(int id);
        Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction);
        Task<FinancialTransaction?> UpdateAsync(int id, FinancialTransaction transaction);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FinancialTransactionResponse>> SearchAsync(string searchTerm);

        // Métodos de dashboard e análises
        Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period);
        Task<object> GetExpensesAsync(DateTime? startDate, DateTime? endDate, string? category);
        Task<object> GetExpenseCategoriesAsync();
        Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetFinancialProjectionsAsync(int months, string type);
        Task<object> CreateProjectionAsync(object projectionData);
        Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType);
        Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period);
        Task<object> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate);
        Task<dynamic> GenerateFinancialReportAsync(string format, DateTime? startDate, DateTime? endDate, string reportType);
    }
} 