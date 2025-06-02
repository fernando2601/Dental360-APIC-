namespace ClinicApi.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Threshold { get; set; }
        public decimal Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Supplier { get; set; }
        public string? SupplierContact { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? LastRestocked { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class InventoryItemWithMetrics
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Threshold { get; set; }
        public decimal Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Supplier { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? LastRestocked { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public int DaysUntilExpiration { get; set; }
        public decimal TotalValue { get; set; }
        public int UsageInLast30Days { get; set; }
        public decimal AverageMonthlyUsage { get; set; }
        public int DaysOfStockRemaining { get; set; }
        public bool NeedsReorder { get; set; }
        public string? LastUsedInProcedure { get; set; }
        public decimal TurnoverRate { get; set; }
    }

    public class CreateInventoryItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Threshold { get; set; }
        public decimal Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Supplier { get; set; }
        public string? SupplierContact { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // in, out, adjustment, expired, damaged
        public int Quantity { get; set; }
        public int? PreviousQuantity { get; set; }
        public int? NewQuantity { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; } // appointment_id, supplier_order, etc.
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class InventoryMovement
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionLabel { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int? PreviousQuantity { get; set; }
        public int? NewQuantity { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class InventoryAnalytics
    {
        public int TotalItems { get; set; }
        public int ActiveItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int ExpiringSoonItems { get; set; }
        public int ExpiredItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageItemValue { get; set; }
        public decimal MonthlyConsumptionValue { get; set; }
        public decimal InventoryTurnoverRate { get; set; }
        public List<CategoryMetrics> CategoryBreakdown { get; set; } = new();
        public List<StockLevelDistribution> StockLevels { get; set; } = new();
        public List<TopUsedItem> TopUsedItems { get; set; } = new();
        public List<ExpirationAlert> ExpirationAlerts { get; set; } = new();
        public List<SupplierMetrics> SupplierPerformance { get; set; } = new();
    }

    public class CategoryMetrics
    {
        public string Category { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal UsageRate { get; set; }
        public decimal Percentage { get; set; }
    }

    public class StockLevelDistribution
    {
        public string Level { get; set; } = string.Empty; // critical, low, normal, high
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class TopUsedItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public decimal UsageValue { get; set; }
        public decimal TurnoverRate { get; set; }
    }

    public class ExpirationAlert
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public int DaysUntilExpiration { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public string AlertLevel { get; set; } = string.Empty; // critical, warning, info
    }

    public class SupplierMetrics
    {
        public string Supplier { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalValue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageDeliveryTime { get; set; }
        public decimal QualityRating { get; set; }
    }

    public class InventoryReorderSuggestion
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public int SuggestedOrderQuantity { get; set; }
        public decimal EstimatedCost { get; set; }
        public string? Supplier { get; set; }
        public int DaysOfStockRemaining { get; set; }
        public decimal UsageRate { get; set; }
        public string Priority { get; set; } = string.Empty; // critical, high, medium, low
    }

    public class InventoryForecast
    {
        public DateTime ForecastDate { get; set; }
        public int PredictedUsage { get; set; }
        public int RequiredStock { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Confidence { get; set; } = string.Empty; // high, medium, low
    }

    public class InventoryAudit
    {
        public int Id { get; set; }
        public DateTime AuditDate { get; set; }
        public string AuditType { get; set; } = string.Empty; // full, partial, cycle
        public int ItemsAudited { get; set; }
        public int Discrepancies { get; set; }
        public decimal VarianceValue { get; set; }
        public string? Notes { get; set; }
        public int ConductedBy { get; set; }
        public string ConductedByName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // pending, completed, approved
    }

    public class InventoryValuation
    {
        public DateTime ValuationDate { get; set; }
        public decimal TotalValue { get; set; }
        public decimal CostValue { get; set; }
        public decimal RetailValue { get; set; }
        public List<CategoryValuation> CategoryBreakdown { get; set; } = new();
        public decimal DeadStockValue { get; set; }
        public decimal FastMovingValue { get; set; }
        public decimal SlowMovingValue { get; set; }
    }

    public class CategoryValuation
    {
        public string Category { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }
        public int ItemCount { get; set; }
    }

    public class InventoryOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // pending, ordered, received, cancelled
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; }
        public decimal TotalValue { get; set; }
        public string? Notes { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public List<InventoryOrderItem> Items { get; set; } = new();
    }

    public class InventoryOrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public int? QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string? Notes { get; set; }
    }

    public class InventoryDashboardMetrics
    {
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockAlerts { get; set; }
        public int ExpirationAlerts { get; set; }
        public int RecentMovements { get; set; }
        public decimal MonthlyConsumption { get; set; }
        public decimal InventoryTurnover { get; set; }
        public List<InventoryAlert> CriticalAlerts { get; set; } = new();
        public List<RecentMovement> RecentTransactions { get; set; } = new();
        public List<TopCategory> TopCategories { get; set; } = new();
    }

    public class InventoryAlert
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty; // low_stock, expired, expiring_soon
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // critical, warning, info
        public DateTime CreatedAt { get; set; }
    }

    public class RecentMovement
    {
        public string ItemName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
    }

    public class TopCategory
    {
        public string Category { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int ItemCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class InventoryExportRequest
    {
        public string Format { get; set; } = string.Empty; // excel, csv, pdf
        public string[]? Categories { get; set; }
        public string[]? Statuses { get; set; }
        public bool IncludeTransactions { get; set; } = false;
        public bool IncludeValuation { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class InventoryBulkAction
    {
        public int[] ItemIds { get; set; } = Array.Empty<int>();
        public string Action { get; set; } = string.Empty; // update_prices, adjust_stock, change_supplier, archive
        public object? Parameters { get; set; }
    }

    public class InventorySettings
    {
        public int DefaultThreshold { get; set; } = 10;
        public int ExpirationWarningDays { get; set; } = 30;
        public bool AutoReorderEnabled { get; set; } = false;
        public decimal DefaultMarkup { get; set; } = 0.5m;
        public string DefaultUnit { get; set; } = "un";
        public bool TrackExpiration { get; set; } = true;
        public bool RequireSupplier { get; set; } = false;
        public bool EnableBarcodeScanning { get; set; } = false;
    }
}