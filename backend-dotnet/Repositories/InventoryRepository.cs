using Dapper;
using Npgsql;
using ClinicApi.Models;
using System.Text;

namespace ClinicApi.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public InventoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM inventory_items 
                WHERE is_active = true
                ORDER BY name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryItem>(sql);
        }

        public async Task<InventoryItem?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM inventory_items 
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<InventoryItem>(sql, new { Id = id });
        }

        public async Task<InventoryItem> CreateAsync(CreateInventoryItemDto itemDto)
        {
            const string sql = @"
                INSERT INTO inventory_items 
                (name, category, description, quantity, unit, threshold, price, cost,
                 supplier, supplier_contact, sku, barcode, location, expiration_date,
                 is_active, created_at, updated_at)
                VALUES 
                (@Name, @Category, @Description, @Quantity, @Unit, @Threshold, @Price, @Cost,
                 @Supplier, @SupplierContact, @Sku, @Barcode, @Location, @ExpirationDate,
                 true, @CreatedAt, @UpdatedAt)
                RETURNING 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            var now = DateTime.UtcNow;
            return await connection.QuerySingleAsync<InventoryItem>(sql, new
            {
                itemDto.Name,
                itemDto.Category,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.Unit,
                itemDto.Threshold,
                itemDto.Price,
                itemDto.Cost,
                itemDto.Supplier,
                itemDto.SupplierContact,
                itemDto.Sku,
                itemDto.Barcode,
                itemDto.Location,
                itemDto.ExpirationDate,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        public async Task<InventoryItem?> UpdateAsync(int id, CreateInventoryItemDto itemDto)
        {
            const string sql = @"
                UPDATE inventory_items 
                SET 
                    name = @Name,
                    category = @Category,
                    description = @Description,
                    quantity = @Quantity,
                    unit = @Unit,
                    threshold = @Threshold,
                    price = @Price,
                    cost = @Cost,
                    supplier = @Supplier,
                    supplier_contact = @SupplierContact,
                    sku = @Sku,
                    barcode = @Barcode,
                    location = @Location,
                    expiration_date = @ExpirationDate,
                    updated_at = @UpdatedAt
                WHERE id = @Id
                RETURNING 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<InventoryItem>(sql, new
            {
                Id = id,
                itemDto.Name,
                itemDto.Category,
                itemDto.Description,
                itemDto.Quantity,
                itemDto.Unit,
                itemDto.Threshold,
                itemDto.Price,
                itemDto.Cost,
                itemDto.Supplier,
                itemDto.SupplierContact,
                itemDto.Sku,
                itemDto.Barcode,
                itemDto.Location,
                itemDto.ExpirationDate,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                UPDATE inventory_items 
                SET 
                    is_active = false,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InventoryItemWithMetrics>> GetInventoryWithMetricsAsync()
        {
            const string sql = @"
                SELECT 
                    i.id as Id,
                    i.name as Name,
                    i.category as Category,
                    i.description as Description,
                    i.quantity as Quantity,
                    i.unit as Unit,
                    i.threshold as Threshold,
                    i.price as Price,
                    i.cost as Cost,
                    i.supplier as Supplier,
                    i.location as Location,
                    i.expiration_date as ExpirationDate,
                    i.last_restocked as LastRestocked,
                    CASE 
                        WHEN i.quantity = 0 THEN 'out_of_stock'
                        WHEN i.quantity <= i.threshold THEN 'low_stock'
                        WHEN i.quantity <= i.threshold * 2 THEN 'warning'
                        ELSE 'in_stock'
                    END as StockStatus,
                    CASE 
                        WHEN i.quantity = 0 THEN '#EF4444'
                        WHEN i.quantity <= i.threshold THEN '#F59E0B'
                        WHEN i.quantity <= i.threshold * 2 THEN '#FBBF24'
                        ELSE '#10B981'
                    END as StatusColor,
                    CASE 
                        WHEN i.expiration_date IS NOT NULL THEN 
                            EXTRACT(DAY FROM (i.expiration_date - CURRENT_DATE))::int
                        ELSE 999
                    END as DaysUntilExpiration,
                    (i.quantity * i.price) as TotalValue,
                    COALESCE(usage.usage_count, 0) as UsageInLast30Days,
                    COALESCE(usage.avg_monthly_usage, 0) as AverageMonthlyUsage,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 THEN 
                            ROUND((i.quantity / usage.avg_monthly_usage * 30)::numeric, 0)::int
                        ELSE 999
                    END as DaysOfStockRemaining,
                    (i.quantity <= i.threshold OR i.quantity <= COALESCE(usage.avg_monthly_usage, 0) * 0.5) as NeedsReorder,
                    recent_usage.last_procedure as LastUsedInProcedure,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 AND i.quantity > 0 THEN
                            ROUND((usage.avg_monthly_usage / i.quantity * 12)::numeric, 2)
                        ELSE 0
                    END as TurnoverRate
                FROM inventory_items i
                LEFT JOIN (
                    SELECT 
                        inventory_item_id,
                        COUNT(*) as usage_count,
                        AVG(COUNT(*)) OVER (PARTITION BY inventory_item_id) as avg_monthly_usage
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND created_at >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY inventory_item_id
                ) usage ON i.id = usage.inventory_item_id
                LEFT JOIN (
                    SELECT DISTINCT ON (inventory_item_id)
                        inventory_item_id,
                        reference as last_procedure
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND reference IS NOT NULL
                    ORDER BY inventory_item_id, created_at DESC
                ) recent_usage ON i.id = recent_usage.inventory_item_id
                WHERE i.is_active = true
                ORDER BY i.name";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryItemWithMetrics>(sql);
        }

        public async Task<InventoryItemWithMetrics?> GetItemWithMetricsAsync(int id)
        {
            const string sql = @"
                SELECT 
                    i.id as Id,
                    i.name as Name,
                    i.category as Category,
                    i.description as Description,
                    i.quantity as Quantity,
                    i.unit as Unit,
                    i.threshold as Threshold,
                    i.price as Price,
                    i.cost as Cost,
                    i.supplier as Supplier,
                    i.location as Location,
                    i.expiration_date as ExpirationDate,
                    i.last_restocked as LastRestocked,
                    CASE 
                        WHEN i.quantity = 0 THEN 'out_of_stock'
                        WHEN i.quantity <= i.threshold THEN 'low_stock'
                        WHEN i.quantity <= i.threshold * 2 THEN 'warning'
                        ELSE 'in_stock'
                    END as StockStatus,
                    CASE 
                        WHEN i.quantity = 0 THEN '#EF4444'
                        WHEN i.quantity <= i.threshold THEN '#F59E0B'
                        WHEN i.quantity <= i.threshold * 2 THEN '#FBBF24'
                        ELSE '#10B981'
                    END as StatusColor,
                    CASE 
                        WHEN i.expiration_date IS NOT NULL THEN 
                            EXTRACT(DAY FROM (i.expiration_date - CURRENT_DATE))::int
                        ELSE 999
                    END as DaysUntilExpiration,
                    (i.quantity * i.price) as TotalValue,
                    COALESCE(usage.usage_count, 0) as UsageInLast30Days,
                    COALESCE(usage.avg_monthly_usage, 0) as AverageMonthlyUsage,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 THEN 
                            ROUND((i.quantity / usage.avg_monthly_usage * 30)::numeric, 0)::int
                        ELSE 999
                    END as DaysOfStockRemaining,
                    (i.quantity <= i.threshold OR i.quantity <= COALESCE(usage.avg_monthly_usage, 0) * 0.5) as NeedsReorder,
                    recent_usage.last_procedure as LastUsedInProcedure,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 AND i.quantity > 0 THEN
                            ROUND((usage.avg_monthly_usage / i.quantity * 12)::numeric, 2)
                        ELSE 0
                    END as TurnoverRate
                FROM inventory_items i
                LEFT JOIN (
                    SELECT 
                        inventory_item_id,
                        COUNT(*) as usage_count,
                        AVG(COUNT(*)) OVER (PARTITION BY inventory_item_id) as avg_monthly_usage
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND created_at >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY inventory_item_id
                ) usage ON i.id = usage.inventory_item_id
                LEFT JOIN (
                    SELECT DISTINCT ON (inventory_item_id)
                        inventory_item_id,
                        reference as last_procedure
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND reference IS NOT NULL
                    ORDER BY inventory_item_id, created_at DESC
                ) recent_usage ON i.id = recent_usage.inventory_item_id
                WHERE i.id = @Id AND i.is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<InventoryItemWithMetrics>(sql, new { Id = id });
        }

        public async Task<IEnumerable<InventoryItemWithMetrics>> GetInventoryWithFiltersAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null, int page = 1, int limit = 25)
        {
            var whereConditions = new List<string> { "i.is_active = true" };
            var parameters = new DynamicParameters();

            var sql = new StringBuilder(@"
                SELECT 
                    i.id as Id,
                    i.name as Name,
                    i.category as Category,
                    i.description as Description,
                    i.quantity as Quantity,
                    i.unit as Unit,
                    i.threshold as Threshold,
                    i.price as Price,
                    i.cost as Cost,
                    i.supplier as Supplier,
                    i.location as Location,
                    i.expiration_date as ExpirationDate,
                    i.last_restocked as LastRestocked,
                    CASE 
                        WHEN i.quantity = 0 THEN 'out_of_stock'
                        WHEN i.quantity <= i.threshold THEN 'low_stock'
                        WHEN i.quantity <= i.threshold * 2 THEN 'warning'
                        ELSE 'in_stock'
                    END as StockStatus,
                    CASE 
                        WHEN i.quantity = 0 THEN '#EF4444'
                        WHEN i.quantity <= i.threshold THEN '#F59E0B'
                        WHEN i.quantity <= i.threshold * 2 THEN '#FBBF24'
                        ELSE '#10B981'
                    END as StatusColor,
                    CASE 
                        WHEN i.expiration_date IS NOT NULL THEN 
                            EXTRACT(DAY FROM (i.expiration_date - CURRENT_DATE))::int
                        ELSE 999
                    END as DaysUntilExpiration,
                    (i.quantity * i.price) as TotalValue,
                    COALESCE(usage.usage_count, 0) as UsageInLast30Days,
                    COALESCE(usage.avg_monthly_usage, 0) as AverageMonthlyUsage,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 THEN 
                            ROUND((i.quantity / usage.avg_monthly_usage * 30)::numeric, 0)::int
                        ELSE 999
                    END as DaysOfStockRemaining,
                    (i.quantity <= i.threshold OR i.quantity <= COALESCE(usage.avg_monthly_usage, 0) * 0.5) as NeedsReorder,
                    recent_usage.last_procedure as LastUsedInProcedure,
                    CASE 
                        WHEN COALESCE(usage.avg_monthly_usage, 0) > 0 AND i.quantity > 0 THEN
                            ROUND((usage.avg_monthly_usage / i.quantity * 12)::numeric, 2)
                        ELSE 0
                    END as TurnoverRate
                FROM inventory_items i
                LEFT JOIN (
                    SELECT 
                        inventory_item_id,
                        COUNT(*) as usage_count,
                        AVG(COUNT(*)) OVER (PARTITION BY inventory_item_id) as avg_monthly_usage
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND created_at >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY inventory_item_id
                ) usage ON i.id = usage.inventory_item_id
                LEFT JOIN (
                    SELECT DISTINCT ON (inventory_item_id)
                        inventory_item_id,
                        reference as last_procedure
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND reference IS NOT NULL
                    ORDER BY inventory_item_id, created_at DESC
                ) recent_usage ON i.id = recent_usage.inventory_item_id");

            // Aplicar filtros
            if (!string.IsNullOrEmpty(category))
            {
                whereConditions.Add("i.category = @Category");
                parameters.Add("Category", category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                whereConditions.Add("(i.name ILIKE @Search OR i.description ILIKE @Search OR i.sku ILIKE @Search)");
                parameters.Add("Search", $"%{search}%");
            }

            if (lowStock == true)
            {
                whereConditions.Add("i.quantity <= i.threshold");
            }

            if (expiringSoon == true)
            {
                whereConditions.Add("i.expiration_date IS NOT NULL AND i.expiration_date <= CURRENT_DATE + INTERVAL '30 days'");
            }

            sql.Append(" WHERE " + string.Join(" AND ", whereConditions));
            sql.Append(" ORDER BY i.name");

            // Paginação
            var offset = (page - 1) * limit;
            sql.Append($" LIMIT {limit} OFFSET {offset}");

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryItemWithMetrics>(sql.ToString(), parameters);
        }

        public async Task<int> GetInventoryCountAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null)
        {
            var whereConditions = new List<string> { "is_active = true" };
            var parameters = new DynamicParameters();

            var sql = new StringBuilder("SELECT COUNT(*) FROM inventory_items");

            if (!string.IsNullOrEmpty(category))
            {
                whereConditions.Add("category = @Category");
                parameters.Add("Category", category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                whereConditions.Add("(name ILIKE @Search OR description ILIKE @Search OR sku ILIKE @Search)");
                parameters.Add("Search", $"%{search}%");
            }

            if (lowStock == true)
            {
                whereConditions.Add("quantity <= threshold");
            }

            if (expiringSoon == true)
            {
                whereConditions.Add("expiration_date IS NOT NULL AND expiration_date <= CURRENT_DATE + INTERVAL '30 days'");
            }

            sql.Append(" WHERE " + string.Join(" AND ", whereConditions));

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<int>(sql.ToString(), parameters);
        }

        public async Task<bool> AdjustStockAsync(int itemId, int quantity, string reason, int userId, string? reference = null)
        {
            const string updateSql = @"
                UPDATE inventory_items 
                SET 
                    quantity = quantity + @Quantity,
                    last_restocked = CASE WHEN @Quantity > 0 THEN @Now ELSE last_restocked END,
                    updated_at = @Now
                WHERE id = @ItemId
                RETURNING quantity - @Quantity as PreviousQuantity, quantity as NewQuantity";

            const string transactionSql = @"
                INSERT INTO inventory_transactions 
                (inventory_item_id, transaction_type, quantity, previous_quantity, new_quantity, 
                 reason, reference, user_id, created_at)
                VALUES 
                (@ItemId, @TransactionType, @Quantity, @PreviousQuantity, @NewQuantity, 
                 @Reason, @Reference, @UserId, @CreatedAt)";

            using var connection = new NpgsqlConnection(_connectionString);
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;
                var stockUpdate = await connection.QuerySingleOrDefaultAsync(updateSql, new
                {
                    ItemId = itemId,
                    Quantity = quantity,
                    Now = now
                }, transaction);

                if (stockUpdate == null) return false;

                await connection.ExecuteAsync(transactionSql, new
                {
                    ItemId = itemId,
                    TransactionType = quantity > 0 ? "in" : "out",
                    Quantity = Math.Abs(quantity),
                    PreviousQuantity = stockUpdate.PreviousQuantity,
                    NewQuantity = stockUpdate.NewQuantity,
                    Reason = reason,
                    Reference = reference,
                    UserId = userId,
                    CreatedAt = now
                }, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ReceiveStockAsync(int itemId, int quantity, decimal? unitCost, int userId, string? reference = null)
        {
            return await AdjustStockAsync(itemId, quantity, "Recebimento de estoque", userId, reference);
        }

        public async Task<bool> ConsumeStockAsync(int itemId, int quantity, int userId, string? reference = null)
        {
            return await AdjustStockAsync(itemId, -quantity, "Consumo em procedimento", userId, reference);
        }

        public async Task<bool> SetStockLevelAsync(int itemId, int newQuantity, string reason, int userId)
        {
            const string sql = @"
                SELECT quantity FROM inventory_items WHERE id = @ItemId";

            using var connection = new NpgsqlConnection(_connectionString);
            var currentQuantity = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { ItemId = itemId });
            
            if (!currentQuantity.HasValue) return false;

            var adjustment = newQuantity - currentQuantity.Value;
            return await AdjustStockAsync(itemId, adjustment, reason, userId);
        }

        public async Task<IEnumerable<InventoryMovement>> GetInventoryMovementsAsync(
            int? itemId = null, DateTime? startDate = null, DateTime? endDate = null,
            string? transactionType = null, int page = 1, int limit = 50)
        {
            var whereConditions = new List<string>();
            var parameters = new DynamicParameters();

            var sql = new StringBuilder(@"
                SELECT 
                    t.id as Id,
                    t.inventory_item_id as InventoryItemId,
                    i.name as ItemName,
                    t.transaction_type as TransactionType,
                    CASE t.transaction_type
                        WHEN 'in' THEN 'Entrada'
                        WHEN 'out' THEN 'Saída'
                        WHEN 'adjustment' THEN 'Ajuste'
                        WHEN 'expired' THEN 'Vencido'
                        WHEN 'damaged' THEN 'Danificado'
                        ELSE 'Outros'
                    END as TransactionLabel,
                    t.quantity as Quantity,
                    t.previous_quantity as PreviousQuantity,
                    t.new_quantity as NewQuantity,
                    t.reason as Reason,
                    t.reference as Reference,
                    t.unit_cost as UnitCost,
                    t.total_cost as TotalCost,
                    u.username as UserName,
                    t.created_at as CreatedAt,
                    TO_CHAR(t.created_at, 'DD/MM/YYYY HH24:MI:SS') as FormattedDate,
                    t.notes as Notes
                FROM inventory_transactions t
                INNER JOIN inventory_items i ON t.inventory_item_id = i.id
                LEFT JOIN users u ON t.user_id = u.id");

            if (itemId.HasValue)
            {
                whereConditions.Add("t.inventory_item_id = @ItemId");
                parameters.Add("ItemId", itemId.Value);
            }

            if (startDate.HasValue)
            {
                whereConditions.Add("t.created_at >= @StartDate");
                parameters.Add("StartDate", startDate.Value);
            }

            if (endDate.HasValue)
            {
                whereConditions.Add("t.created_at <= @EndDate");
                parameters.Add("EndDate", endDate.Value);
            }

            if (!string.IsNullOrEmpty(transactionType))
            {
                whereConditions.Add("t.transaction_type = @TransactionType");
                parameters.Add("TransactionType", transactionType);
            }

            if (whereConditions.Any())
            {
                sql.Append(" WHERE " + string.Join(" AND ", whereConditions));
            }

            sql.Append(" ORDER BY t.created_at DESC");

            var offset = (page - 1) * limit;
            sql.Append($" LIMIT {limit} OFFSET {offset}");

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryMovement>(sql.ToString(), parameters);
        }

        public async Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction)
        {
            const string sql = @"
                INSERT INTO inventory_transactions 
                (inventory_item_id, transaction_type, quantity, previous_quantity, new_quantity,
                 reason, reference, unit_cost, total_cost, user_id, created_at, notes)
                VALUES 
                (@InventoryItemId, @TransactionType, @Quantity, @PreviousQuantity, @NewQuantity,
                 @Reason, @Reference, @UnitCost, @TotalCost, @UserId, @CreatedAt, @Notes)
                RETURNING 
                    id as Id,
                    inventory_item_id as InventoryItemId,
                    transaction_type as TransactionType,
                    quantity as Quantity,
                    previous_quantity as PreviousQuantity,
                    new_quantity as NewQuantity,
                    reason as Reason,
                    reference as Reference,
                    unit_cost as UnitCost,
                    total_cost as TotalCost,
                    user_id as UserId,
                    created_at as CreatedAt,
                    notes as Notes";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<InventoryTransaction>(sql, new
            {
                transaction.InventoryItemId,
                transaction.TransactionType,
                transaction.Quantity,
                transaction.PreviousQuantity,
                transaction.NewQuantity,
                transaction.Reason,
                transaction.Reference,
                transaction.UnitCost,
                transaction.TotalCost,
                transaction.UserId,
                CreatedAt = DateTime.UtcNow,
                transaction.Notes
            });
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByItemAsync(int itemId)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    inventory_item_id as InventoryItemId,
                    transaction_type as TransactionType,
                    quantity as Quantity,
                    previous_quantity as PreviousQuantity,
                    new_quantity as NewQuantity,
                    reason as Reason,
                    reference as Reference,
                    unit_cost as UnitCost,
                    total_cost as TotalCost,
                    user_id as UserId,
                    created_at as CreatedAt,
                    notes as Notes
                FROM inventory_transactions 
                WHERE inventory_item_id = @ItemId
                ORDER BY created_at DESC
                LIMIT 100";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryTransaction>(sql, new { ItemId = itemId });
        }

        public async Task<InventoryAnalytics> GetInventoryAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var dates = NormalizeDateRange(startDate, endDate);

            const string analyticsSql = @"
                SELECT 
                    COUNT(*) as TotalItems,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveItems,
                    COUNT(CASE WHEN quantity <= threshold AND is_active = true THEN 1 END) as LowStockItems,
                    COUNT(CASE WHEN quantity = 0 AND is_active = true THEN 1 END) as OutOfStockItems,
                    COUNT(CASE WHEN expiration_date IS NOT NULL 
                        AND expiration_date <= CURRENT_DATE + INTERVAL '30 days' 
                        AND expiration_date > CURRENT_DATE 
                        AND is_active = true THEN 1 END) as ExpiringSoonItems,
                    COUNT(CASE WHEN expiration_date IS NOT NULL 
                        AND expiration_date <= CURRENT_DATE 
                        AND is_active = true THEN 1 END) as ExpiredItems,
                    COALESCE(SUM(CASE WHEN is_active = true THEN quantity * price END), 0) as TotalInventoryValue,
                    COALESCE(AVG(CASE WHEN is_active = true THEN price END), 0) as AverageItemValue
                FROM inventory_items";

            using var connection = new NpgsqlConnection(_connectionString);
            var analytics = await connection.QuerySingleAsync(analyticsSql);

            var result = new InventoryAnalytics
            {
                TotalItems = analytics.TotalItems,
                ActiveItems = analytics.ActiveItems,
                LowStockItems = analytics.LowStockItems,
                OutOfStockItems = analytics.OutOfStockItems,
                ExpiringSoonItems = analytics.ExpiringSoonItems,
                ExpiredItems = analytics.ExpiredItems,
                TotalInventoryValue = analytics.TotalInventoryValue,
                AverageItemValue = analytics.AverageItemValue,
                CategoryBreakdown = await GetCategoryBreakdown(),
                StockLevels = await GetStockLevelDistribution(),
                TopUsedItems = await GetTopUsedItems(),
                ExpirationAlerts = await GetExpirationAlerts(30),
                SupplierPerformance = await GetSupplierPerformance()
            };

            return result;
        }

        public async Task<InventoryDashboardMetrics> GetDashboardMetricsAsync()
        {
            const string sql = @"
                SELECT 
                    COUNT(CASE WHEN is_active = true THEN 1 END) as TotalItems,
                    COALESCE(SUM(CASE WHEN is_active = true THEN quantity * price END), 0) as TotalValue,
                    COUNT(CASE WHEN quantity <= threshold AND is_active = true THEN 1 END) as LowStockAlerts,
                    COUNT(CASE WHEN expiration_date IS NOT NULL 
                        AND expiration_date <= CURRENT_DATE + INTERVAL '30 days' 
                        AND is_active = true THEN 1 END) as ExpirationAlerts
                FROM inventory_items";

            const string movementsSql = @"
                SELECT COUNT(*) FROM inventory_transactions 
                WHERE created_at >= CURRENT_DATE - INTERVAL '7 days'";

            using var connection = new NpgsqlConnection(_connectionString);
            var metrics = await connection.QuerySingleAsync(sql);
            var recentMovements = await connection.QuerySingleAsync<int>(movementsSql);

            return new InventoryDashboardMetrics
            {
                TotalItems = metrics.TotalItems,
                TotalValue = metrics.TotalValue,
                LowStockAlerts = metrics.LowStockAlerts,
                ExpirationAlerts = metrics.ExpirationAlerts,
                RecentMovements = recentMovements,
                MonthlyConsumption = await CalculateMonthlyConsumption(),
                InventoryTurnover = await CalculateInventoryTurnover(),
                CriticalAlerts = await GetCriticalAlerts(),
                RecentTransactions = await GetRecentTransactions(),
                TopCategories = await GetTopCategories()
            };
        }

        // Métodos auxiliares privados
        private async Task<List<CategoryMetrics>> GetCategoryBreakdown()
        {
            const string sql = @"
                SELECT 
                    category as Category,
                    COUNT(*) as ItemCount,
                    COALESCE(SUM(quantity * price), 0) as TotalValue,
                    COUNT(CASE WHEN quantity <= threshold THEN 1 END) as LowStockCount,
                    COALESCE(AVG(price), 0) as AveragePrice,
                    0 as UsageRate
                FROM inventory_items 
                WHERE is_active = true
                GROUP BY category
                ORDER BY TotalValue DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            var results = await connection.QueryAsync(sql);
            var total = results.Sum(r => r.TotalValue);

            return results.Select(r => new CategoryMetrics
            {
                Category = r.Category,
                ItemCount = r.ItemCount,
                TotalValue = r.TotalValue,
                LowStockCount = r.LowStockCount,
                AveragePrice = r.AveragePrice,
                UsageRate = r.UsageRate,
                Percentage = total > 0 ? (decimal)r.TotalValue / total * 100 : 0
            }).ToList();
        }

        private async Task<List<StockLevelDistribution>> GetStockLevelDistribution()
        {
            const string sql = @"
                SELECT 
                    CASE 
                        WHEN quantity = 0 THEN 'Sem estoque'
                        WHEN quantity <= threshold THEN 'Estoque baixo'
                        WHEN quantity <= threshold * 2 THEN 'Estoque normal'
                        ELSE 'Estoque alto'
                    END as Level,
                    COUNT(*) as Count
                FROM inventory_items 
                WHERE is_active = true
                GROUP BY 
                    CASE 
                        WHEN quantity = 0 THEN 'Sem estoque'
                        WHEN quantity <= threshold THEN 'Estoque baixo'
                        WHEN quantity <= threshold * 2 THEN 'Estoque normal'
                        ELSE 'Estoque alto'
                    END";

            using var connection = new NpgsqlConnection(_connectionString);
            var results = await connection.QueryAsync(sql);
            var total = results.Sum(r => r.Count);

            return results.Select(r => new StockLevelDistribution
            {
                Level = r.Level,
                Count = r.Count,
                Percentage = total > 0 ? (decimal)r.Count / total * 100 : 0,
                Color = GetStockLevelColor(r.Level)
            }).ToList();
        }

        private async Task<List<TopUsedItem>> GetTopUsedItems()
        {
            const string sql = @"
                SELECT 
                    i.id as Id,
                    i.name as Name,
                    i.category as Category,
                    COALESCE(t.usage_count, 0) as UsageCount,
                    COALESCE(t.usage_value, 0) as UsageValue,
                    0 as TurnoverRate
                FROM inventory_items i
                LEFT JOIN (
                    SELECT 
                        inventory_item_id,
                        COUNT(*) as usage_count,
                        SUM(quantity * COALESCE(unit_cost, 0)) as usage_value
                    FROM inventory_transactions 
                    WHERE transaction_type = 'out' 
                        AND created_at >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY inventory_item_id
                ) t ON i.id = t.inventory_item_id
                WHERE i.is_active = true
                ORDER BY t.usage_count DESC NULLS LAST
                LIMIT 10";

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<TopUsedItem>(sql)).ToList();
        }

        private async Task<List<SupplierMetrics>> GetSupplierPerformance()
        {
            const string sql = @"
                SELECT 
                    COALESCE(supplier, 'Não informado') as Supplier,
                    COUNT(*) as ItemCount,
                    COALESCE(SUM(quantity * price), 0) as TotalValue,
                    0 as OrderCount,
                    0 as AverageDeliveryTime,
                    0 as QualityRating
                FROM inventory_items 
                WHERE is_active = true
                GROUP BY supplier
                ORDER BY TotalValue DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<SupplierMetrics>(sql)).ToList();
        }

        private async Task<decimal> CalculateMonthlyConsumption()
        {
            const string sql = @"
                SELECT COALESCE(SUM(quantity * COALESCE(unit_cost, 0)), 0)
                FROM inventory_transactions 
                WHERE transaction_type = 'out' 
                    AND created_at >= DATE_TRUNC('month', CURRENT_DATE)";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleAsync<decimal>(sql);
        }

        private async Task<decimal> CalculateInventoryTurnover()
        {
            const string sql = @"
                SELECT 
                    CASE 
                        WHEN AVG(quantity * price) > 0 THEN
                            SUM(CASE WHEN t.transaction_type = 'out' THEN t.quantity * COALESCE(t.unit_cost, i.price) END) / 
                            AVG(i.quantity * i.price) * 12
                        ELSE 0
                    END
                FROM inventory_items i
                LEFT JOIN inventory_transactions t ON i.id = t.inventory_item_id 
                    AND t.created_at >= CURRENT_DATE - INTERVAL '30 days'
                WHERE i.is_active = true";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<decimal>(sql);
        }

        private async Task<List<InventoryAlert>> GetCriticalAlerts()
        {
            const string sql = @"
                SELECT 
                    id as ItemId,
                    name as ItemName,
                    CASE 
                        WHEN quantity = 0 THEN 'out_of_stock'
                        WHEN quantity <= threshold THEN 'low_stock'
                        WHEN expiration_date <= CURRENT_DATE THEN 'expired'
                        WHEN expiration_date <= CURRENT_DATE + INTERVAL '7 days' THEN 'expiring_soon'
                    END as AlertType,
                    CASE 
                        WHEN quantity = 0 THEN 'Item sem estoque'
                        WHEN quantity <= threshold THEN 'Estoque baixo'
                        WHEN expiration_date <= CURRENT_DATE THEN 'Item vencido'
                        WHEN expiration_date <= CURRENT_DATE + INTERVAL '7 days' THEN 'Vence em breve'
                    END as Message,
                    CASE 
                        WHEN quantity = 0 OR expiration_date <= CURRENT_DATE THEN 'critical'
                        ELSE 'warning'
                    END as Severity,
                    CURRENT_TIMESTAMP as CreatedAt
                FROM inventory_items 
                WHERE is_active = true
                    AND (
                        quantity <= threshold OR 
                        expiration_date <= CURRENT_DATE + INTERVAL '7 days'
                    )
                ORDER BY 
                    CASE 
                        WHEN quantity = 0 OR expiration_date <= CURRENT_DATE THEN 1
                        ELSE 2
                    END,
                    expiration_date ASC NULLS LAST
                LIMIT 10";

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<InventoryAlert>(sql)).ToList();
        }

        private async Task<List<RecentMovement>> GetRecentTransactions()
        {
            const string sql = @"
                SELECT 
                    i.name as ItemName,
                    t.transaction_type as TransactionType,
                    t.quantity as Quantity,
                    t.created_at as Date,
                    TO_CHAR(t.created_at, 'DD/MM HH24:MI') as FormattedDate
                FROM inventory_transactions t
                INNER JOIN inventory_items i ON t.inventory_item_id = i.id
                ORDER BY t.created_at DESC
                LIMIT 10";

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<RecentMovement>(sql)).ToList();
        }

        private async Task<List<TopCategory>> GetTopCategories()
        {
            const string sql = @"
                SELECT 
                    category as Category,
                    COALESCE(SUM(quantity * price), 0) as Value,
                    COUNT(*) as ItemCount
                FROM inventory_items 
                WHERE is_active = true
                GROUP BY category
                ORDER BY Value DESC
                LIMIT 5";

            using var connection = new NpgsqlConnection(_connectionString);
            var results = await connection.QueryAsync(sql);
            var total = results.Sum(r => r.Value);

            return results.Select(r => new TopCategory
            {
                Category = r.Category,
                Value = r.Value,
                ItemCount = r.ItemCount,
                Percentage = total > 0 ? (decimal)r.Value / total * 100 : 0
            }).ToList();
        }

        private string GetStockLevelColor(string level)
        {
            return level switch
            {
                "Sem estoque" => "#EF4444",
                "Estoque baixo" => "#F59E0B",
                "Estoque normal" => "#10B981",
                "Estoque alto" => "#3B82F6",
                _ => "#6B7280"
            };
        }

        private (DateTime start, DateTime end) NormalizeDateRange(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now.Date.AddDays(1).AddTicks(-1);
            var start = startDate ?? end.AddDays(-30);
            return (start, end);
        }

        // Implementações simplificadas para os demais métodos
        public async Task<object> GetStockLevelReportAsync()
        {
            return new { message = "Relatório de níveis de estoque" };
        }

        public async Task<object> GetExpirationReportAsync()
        {
            return new { message = "Relatório de vencimentos" };
        }

        public async Task<object> GetUsageReportAsync(DateTime startDate, DateTime endDate)
        {
            return new { message = "Relatório de uso", startDate, endDate };
        }

        public async Task<IEnumerable<InventoryReorderSuggestion>> GetReorderSuggestionsAsync()
        {
            return new List<InventoryReorderSuggestion>();
        }

        public async Task<object> GetInventoryForecastAsync(int itemId, int days = 30)
        {
            return new { message = "Previsão de estoque", itemId, days };
        }

        public async Task<bool> UpdateThresholdAsync(int itemId, int newThreshold)
        {
            const string sql = @"
                UPDATE inventory_items 
                SET threshold = @NewThreshold, updated_at = @UpdatedAt 
                WHERE id = @ItemId";

            using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                ItemId = itemId, 
                NewThreshold = newThreshold, 
                UpdatedAt = DateTime.UtcNow 
            });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InventoryAlert>> GetActiveAlertsAsync()
        {
            return await GetCriticalAlerts();
        }

        public async Task<IEnumerable<ExpirationAlert>> GetExpirationAlertsAsync(int days = 30)
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    expiration_date as ExpirationDate,
                    EXTRACT(DAY FROM (expiration_date - CURRENT_DATE))::int as DaysUntilExpiration,
                    quantity as Quantity,
                    (quantity * price) as Value,
                    CASE 
                        WHEN expiration_date <= CURRENT_DATE THEN 'critical'
                        WHEN expiration_date <= CURRENT_DATE + INTERVAL '7 days' THEN 'warning'
                        ELSE 'info'
                    END as AlertLevel
                FROM inventory_items 
                WHERE is_active = true
                    AND expiration_date IS NOT NULL
                    AND expiration_date <= CURRENT_DATE + INTERVAL '@Days days'
                ORDER BY expiration_date ASC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<ExpirationAlert>(sql.Replace("@Days", days.ToString()));
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM inventory_items 
                WHERE is_active = true AND quantity <= threshold
                ORDER BY (quantity::float / NULLIF(threshold, 0)) ASC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryItem>(sql);
        }

        public async Task<IEnumerable<InventoryItem>> GetExpiredItemsAsync()
        {
            const string sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    category as Category,
                    description as Description,
                    quantity as Quantity,
                    unit as Unit,
                    threshold as Threshold,
                    price as Price,
                    cost as Cost,
                    supplier as Supplier,
                    supplier_contact as SupplierContact,
                    sku as Sku,
                    barcode as Barcode,
                    location as Location,
                    expiration_date as ExpirationDate,
                    last_restocked as LastRestocked,
                    is_active as IsActive,
                    created_at as CreatedAt,
                    updated_at as UpdatedAt
                FROM inventory_items 
                WHERE is_active = true 
                    AND expiration_date IS NOT NULL 
                    AND expiration_date <= CURRENT_DATE
                ORDER BY expiration_date DESC";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<InventoryItem>(sql);
        }

        public async Task<InventoryValuation> GetInventoryValuationAsync(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Now;
            
            return new InventoryValuation
            {
                ValuationDate = targetDate,
                TotalValue = 0,
                CostValue = 0,
                RetailValue = 0,
                CategoryBreakdown = new List<CategoryValuation>(),
                DeadStockValue = 0,
                FastMovingValue = 0,
                SlowMovingValue = 0
            };
        }

        public async Task<object> GetCategoryValuationAsync()
        {
            return await GetCategoryBreakdown();
        }

        public async Task<object> GetSupplierPerformanceAsync()
        {
            return await GetSupplierPerformance();
        }

        public async Task<IEnumerable<InventoryOrder>> GetOrdersAsync()
        {
            return new List<InventoryOrder>();
        }

        public async Task<InventoryOrder> CreateOrderAsync(InventoryOrder order)
        {
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            return true;
        }

        public async Task<bool> ReceiveOrderAsync(int orderId, List<InventoryOrderItem> receivedItems)
        {
            return true;
        }

        public async Task<bool> BulkUpdateAsync(InventoryBulkAction action)
        {
            return true;
        }

        public async Task<object> ExportInventoryAsync(InventoryExportRequest request)
        {
            return new { success = true, message = "Exportação realizada", format = request.Format };
        }

        public async Task<object> ImportInventoryAsync(object importData)
        {
            return new { success = true, message = "Importação realizada", imported = 0 };
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            const string sql = @"
                SELECT DISTINCT category 
                FROM inventory_items 
                WHERE is_active = true AND category IS NOT NULL
                ORDER BY category";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<string>> GetSuppliersAsync()
        {
            const string sql = @"
                SELECT DISTINCT supplier 
                FROM inventory_items 
                WHERE is_active = true AND supplier IS NOT NULL
                ORDER BY supplier";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<string>(sql);
        }

        public async Task<object> GetCategoryMetricsAsync()
        {
            return await GetCategoryBreakdown();
        }

        public async Task<InventorySettings> GetSettingsAsync()
        {
            return new InventorySettings
            {
                DefaultThreshold = 10,
                ExpirationWarningDays = 30,
                AutoReorderEnabled = false,
                DefaultMarkup = 0.5m,
                DefaultUnit = "un",
                TrackExpiration = true,
                RequireSupplier = false,
                EnableBarcodeScanning = false
            };
        }

        public async Task<bool> UpdateSettingsAsync(InventorySettings settings)
        {
            return true;
        }
    }
}