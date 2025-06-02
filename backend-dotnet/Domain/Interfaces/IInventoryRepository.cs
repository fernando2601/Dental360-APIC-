using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        // CRUD Básico
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem?> GetByIdAsync(int id);
        Task<InventoryItem> CreateAsync(CreateInventoryItemDto item);
        Task<InventoryItem?> UpdateAsync(int id, CreateInventoryItemDto item);
        Task<bool> DeleteAsync(int id);

        // Advanced Inventory Views
        Task<IEnumerable<InventoryItemWithMetrics>> GetInventoryWithMetricsAsync();
        Task<InventoryItemWithMetrics?> GetItemWithMetricsAsync(int id);
        Task<IEnumerable<InventoryItemWithMetrics>> GetInventoryWithFiltersAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null, int page = 1, int limit = 25);
        Task<int> GetInventoryCountAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null);

        // Stock Management
        Task<bool> AdjustStockAsync(int itemId, int quantity, string reason, int userId, string? reference = null);
        Task<bool> ReceiveStockAsync(int itemId, int quantity, decimal? unitCost, int userId, string? reference = null);
        Task<bool> ConsumeStockAsync(int itemId, int quantity, int userId, string? reference = null);
        Task<bool> SetStockLevelAsync(int itemId, int newQuantity, string reason, int userId);

        // Transactions & Movements
        Task<IEnumerable<InventoryMovement>> GetInventoryMovementsAsync(
            int? itemId = null, DateTime? startDate = null, DateTime? endDate = null,
            string? transactionType = null, int page = 1, int limit = 50);
        Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByItemAsync(int itemId);

        // Analytics & Reports
        Task<InventoryAnalytics> GetInventoryAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<InventoryDashboardMetrics> GetDashboardMetricsAsync();
        Task<object> GetStockLevelReportAsync();
        Task<object> GetExpirationReportAsync();
        Task<object> GetUsageReportAsync(DateTime startDate, DateTime endDate);

        // Reorder Management
        Task<IEnumerable<InventoryReorderSuggestion>> GetReorderSuggestionsAsync();
        Task<object> GetInventoryForecastAsync(int itemId, int days = 30);
        Task<bool> UpdateThresholdAsync(int itemId, int newThreshold);

        // Alerts & Notifications
        Task<IEnumerable<InventoryAlert>> GetActiveAlertsAsync();
        Task<IEnumerable<ExpirationAlert>> GetExpirationAlertsAsync(int days = 30);
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
        Task<IEnumerable<InventoryItem>> GetExpiredItemsAsync();

        // Valuation & Auditing
        Task<InventoryValuation> GetInventoryValuationAsync(DateTime? date = null);
        Task<object> GetCategoryValuationAsync();
        Task<object> GetSupplierPerformanceAsync();

        // Orders & Procurement
        Task<IEnumerable<InventoryOrder>> GetOrdersAsync();
        Task<InventoryOrder> CreateOrderAsync(InventoryOrder order);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> ReceiveOrderAsync(int orderId, List<InventoryOrderItem> receivedItems);

        // Bulk Operations
        Task<bool> BulkUpdateAsync(InventoryBulkAction action);
        Task<object> ExportInventoryAsync(InventoryExportRequest request);
        Task<object> ImportInventoryAsync(object importData);

        // Categories & Suppliers
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetSuppliersAsync();
        Task<object> GetCategoryMetricsAsync();

        // Settings
        Task<InventorySettings> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(InventorySettings settings);
    }
}