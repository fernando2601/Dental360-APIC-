using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IInventoryService
    {
        // CRUD Básico
        Task<IEnumerable<InventoryItem>> GetAllInventoryAsync();
        Task<InventoryItem?> GetInventoryItemByIdAsync(int id);
        Task<InventoryItem> CreateInventoryItemAsync(CreateInventoryItemDto item);
        Task<InventoryItem?> UpdateInventoryItemAsync(int id, CreateInventoryItemDto item);
        Task<bool> DeleteInventoryItemAsync(int id);

        // Advanced Inventory Management
        Task<object> GetInventoryWithMetricsAsync();
        Task<object> GetInventoryWithFiltersAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null, int page = 1, int limit = 25);

        // Stock Management
        Task<bool> AdjustStockAsync(int itemId, int quantity, string reason, int userId, string? reference = null);
        Task<bool> ReceiveStockAsync(int itemId, int quantity, decimal? unitCost, int userId, string? reference = null);
        Task<bool> ConsumeStockAsync(int itemId, int quantity, int userId, string? reference = null);
        Task<bool> SetStockLevelAsync(int itemId, int newQuantity, string reason, int userId);

        // Transactions & Movements
        Task<object> GetInventoryMovementsAsync(
            int? itemId = null, DateTime? startDate = null, DateTime? endDate = null,
            string? transactionType = null, int page = 1, int limit = 50);
        Task<object> GetItemTransactionHistoryAsync(int itemId);

        // Analytics & Reports
        Task<InventoryAnalytics> GetInventoryAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<InventoryDashboardMetrics> GetDashboardMetricsAsync();
        Task<object> GetStockLevelReportAsync();
        Task<object> GetExpirationReportAsync();
        Task<object> GetUsageReportAsync(DateTime startDate, DateTime endDate);

        // Alerts & Monitoring
        Task<object> GetInventoryAlertsAsync();
        Task<object> GetLowStockAlertsAsync();
        Task<object> GetExpirationAlertsAsync(int days = 30);
        Task<object> GetCriticalAlertsAsync();

        // Reorder Management
        Task<object> GetReorderSuggestionsAsync();
        Task<object> GetInventoryForecastAsync(int itemId, int days = 30);
        Task<bool> UpdateThresholdAsync(int itemId, int newThreshold);

        // Valuation & Auditing
        Task<object> GetInventoryValuationAsync(DateTime? date = null);
        Task<object> GetCategoryAnalysisAsync();
        Task<object> GetSupplierPerformanceAsync();

        // Bulk Operations
        Task<object> BulkUpdateInventoryAsync(InventoryBulkAction action);
        Task<object> ExportInventoryAsync(InventoryExportRequest request);
        Task<object> ImportInventoryAsync(object importData);

        // Categories & Suppliers
        Task<object> GetCategoriesAsync();
        Task<object> GetSuppliersAsync();
        Task<object> GetCategoryMetricsAsync();

        // Settings & Configuration
        Task<InventorySettings> GetInventorySettingsAsync();
        Task<bool> UpdateInventorySettingsAsync(InventorySettings settings);

        // Advanced Features
        Task<object> GetInventoryOptimizationAsync();
        Task<object> GetTurnoverAnalysisAsync();
        Task<object> GetDeadStockAnalysisAsync();
    }
}