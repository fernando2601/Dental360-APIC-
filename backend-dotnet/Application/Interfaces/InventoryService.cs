using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllInventoryAsync()
        {
            return await _inventoryRepository.GetAllAsync();
        }

        public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
        {
            return await _inventoryRepository.GetByIdAsync(id);
        }

        public async Task<InventoryItem> CreateInventoryItemAsync(CreateInventoryItemDto itemDto)
        {
            // Validações de negócio
            await ValidateInventoryItemAsync(itemDto);
            
            // Verificar se já existe item com mesmo nome/SKU
            if (!string.IsNullOrEmpty(itemDto.Sku))
            {
                var existingItems = await _inventoryRepository.GetInventoryWithFiltersAsync(search: itemDto.Sku);
                if (existingItems.Any())
                {
                    throw new InvalidOperationException("Já existe um item com este SKU");
                }
            }

            return await _inventoryRepository.CreateAsync(itemDto);
        }

        public async Task<InventoryItem?> UpdateInventoryItemAsync(int id, CreateInventoryItemDto itemDto)
        {
            await ValidateInventoryItemAsync(itemDto);
            return await _inventoryRepository.UpdateAsync(id, itemDto);
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
                return false;

            // Verificar se pode deletar (não tem movimentações recentes)
            return await _inventoryRepository.DeleteAsync(id);
        }

        public async Task<object> GetInventoryWithMetricsAsync()
        {
            var inventoryWithMetrics = await _inventoryRepository.GetInventoryWithMetricsAsync();
            
            return new
            {
                inventory = inventoryWithMetrics.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    category = item.Category,
                    description = item.Description,
                    quantity = item.Quantity,
                    unit = item.Unit,
                    threshold = item.Threshold,
                    price = item.Price,
                    cost = item.Cost,
                    supplier = item.Supplier,
                    location = item.Location,
                    expirationDate = item.ExpirationDate?.ToString("yyyy-MM-dd"),
                    lastRestocked = item.LastRestocked?.ToString("yyyy-MM-dd"),
                    stockStatus = new
                    {
                        status = item.StockStatus,
                        label = GetStockStatusLabel(item.StockStatus),
                        color = item.StatusColor,
                        needsReorder = item.NeedsReorder
                    },
                    metrics = new
                    {
                        daysUntilExpiration = item.DaysUntilExpiration,
                        totalValue = item.TotalValue,
                        usageInLast30Days = item.UsageInLast30Days,
                        averageMonthlyUsage = item.AverageMonthlyUsage,
                        daysOfStockRemaining = item.DaysOfStockRemaining,
                        turnoverRate = item.TurnoverRate,
                        lastUsedInProcedure = item.LastUsedInProcedure
                    }
                }),
                summary = new
                {
                    totalItems = inventoryWithMetrics.Count(),
                    totalValue = inventoryWithMetrics.Sum(i => i.TotalValue),
                    lowStockItems = inventoryWithMetrics.Count(i => i.StockStatus == "low_stock"),
                    outOfStockItems = inventoryWithMetrics.Count(i => i.StockStatus == "out_of_stock"),
                    expiringSoonItems = inventoryWithMetrics.Count(i => i.DaysUntilExpiration <= 30 && i.DaysUntilExpiration > 0),
                    needsReorderCount = inventoryWithMetrics.Count(i => i.NeedsReorder)
                }
            };
        }

        public async Task<object> GetInventoryWithFiltersAsync(
            string? category = null, string? status = null, string? search = null,
            bool? lowStock = null, bool? expiringSoon = null, int page = 1, int limit = 25)
        {
            var inventory = await _inventoryRepository.GetInventoryWithFiltersAsync(
                category, status, search, lowStock, expiringSoon, page, limit);
            
            var totalCount = await _inventoryRepository.GetInventoryCountAsync(
                category, status, search, lowStock, expiringSoon);

            return new
            {
                inventory = inventory.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    category = item.Category,
                    description = item.Description,
                    quantity = item.Quantity,
                    unit = item.Unit,
                    threshold = item.Threshold,
                    price = item.Price,
                    cost = item.Cost,
                    supplier = item.Supplier,
                    location = item.Location,
                    expirationDate = item.ExpirationDate?.ToString("yyyy-MM-dd"),
                    lastRestocked = item.LastRestocked?.ToString("yyyy-MM-dd"),
                    stockStatus = new
                    {
                        status = item.StockStatus,
                        label = GetStockStatusLabel(item.StockStatus),
                        color = item.StatusColor,
                        needsReorder = item.NeedsReorder
                    },
                    metrics = new
                    {
                        daysUntilExpiration = item.DaysUntilExpiration,
                        totalValue = item.TotalValue,
                        usageInLast30Days = item.UsageInLast30Days,
                        turnoverRate = item.TurnoverRate
                    }
                }),
                pagination = new
                {
                    currentPage = page,
                    totalPages = (int)Math.Ceiling((double)totalCount / limit),
                    totalItems = totalCount,
                    itemsPerPage = limit,
                    hasNext = page * limit < totalCount,
                    hasPrevious = page > 1
                },
                filters = new
                {
                    category,
                    status,
                    search,
                    lowStock,
                    expiringSoon
                }
            };
        }

        public async Task<bool> AdjustStockAsync(int itemId, int quantity, string reason, int userId, string? reference = null)
        {
            // Validar ajuste
            if (quantity == 0)
                throw new ArgumentException("Quantidade de ajuste não pode ser zero");

            var item = await _inventoryRepository.GetByIdAsync(itemId);
            if (item == null)
                throw new ArgumentException("Item não encontrado");

            if (item.Quantity + quantity < 0)
                throw new ArgumentException("Ajuste resultaria em estoque negativo");

            return await _inventoryRepository.AdjustStockAsync(itemId, quantity, reason, userId, reference);
        }

        public async Task<bool> ReceiveStockAsync(int itemId, int quantity, decimal? unitCost, int userId, string? reference = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantidade recebida deve ser positiva");

            return await _inventoryRepository.ReceiveStockAsync(itemId, quantity, unitCost, userId, reference);
        }

        public async Task<bool> ConsumeStockAsync(int itemId, int quantity, int userId, string? reference = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantidade consumida deve ser positiva");

            var item = await _inventoryRepository.GetByIdAsync(itemId);
            if (item == null)
                throw new ArgumentException("Item não encontrado");

            if (item.Quantity < quantity)
                throw new ArgumentException("Estoque insuficiente para consumo");

            return await _inventoryRepository.ConsumeStockAsync(itemId, quantity, userId, reference);
        }

        public async Task<bool> SetStockLevelAsync(int itemId, int newQuantity, string reason, int userId)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Quantidade não pode ser negativa");

            return await _inventoryRepository.SetStockLevelAsync(itemId, newQuantity, reason, userId);
        }

        public async Task<object> GetInventoryMovementsAsync(
            int? itemId = null, DateTime? startDate = null, DateTime? endDate = null,
            string? transactionType = null, int page = 1, int limit = 50)
        {
            var movements = await _inventoryRepository.GetInventoryMovementsAsync(
                itemId, startDate, endDate, transactionType, page, limit);

            return new
            {
                movements = movements.Select(m => new
                {
                    id = m.Id,
                    itemId = m.InventoryItemId,
                    itemName = m.ItemName,
                    transactionType = m.TransactionType,
                    transactionLabel = m.TransactionLabel,
                    quantity = m.Quantity,
                    previousQuantity = m.PreviousQuantity,
                    newQuantity = m.NewQuantity,
                    reason = m.Reason,
                    reference = m.Reference,
                    unitCost = m.UnitCost,
                    totalCost = m.TotalCost,
                    userName = m.UserName,
                    createdAt = m.CreatedAt,
                    formattedDate = m.FormattedDate,
                    notes = m.Notes
                }),
                summary = new
                {
                    totalMovements = movements.Count(),
                    inboundCount = movements.Count(m => m.TransactionType == "in"),
                    outboundCount = movements.Count(m => m.TransactionType == "out"),
                    adjustmentCount = movements.Count(m => m.TransactionType == "adjustment"),
                    period = new
                    {
                        startDate = startDate?.ToString("yyyy-MM-dd"),
                        endDate = endDate?.ToString("yyyy-MM-dd")
                    }
                }
            };
        }

        public async Task<object> GetItemTransactionHistoryAsync(int itemId)
        {
            var transactions = await _inventoryRepository.GetTransactionsByItemAsync(itemId);
            var item = await _inventoryRepository.GetByIdAsync(itemId);

            return new
            {
                itemId,
                itemName = item?.Name,
                transactions = transactions.Select(t => new
                {
                    id = t.Id,
                    transactionType = t.TransactionType,
                    quantity = t.Quantity,
                    previousQuantity = t.PreviousQuantity,
                    newQuantity = t.NewQuantity,
                    reason = t.Reason,
                    reference = t.Reference,
                    unitCost = t.UnitCost,
                    totalCost = t.TotalCost,
                    createdAt = t.CreatedAt,
                    notes = t.Notes
                }).OrderByDescending(t => t.createdAt),
                summary = new
                {
                    totalTransactions = transactions.Count(),
                    totalIn = transactions.Where(t => t.TransactionType == "in").Sum(t => t.Quantity),
                    totalOut = transactions.Where(t => t.TransactionType == "out").Sum(t => t.Quantity),
                    currentQuantity = item?.Quantity ?? 0
                }
            };
        }

        public async Task<InventoryAnalytics> GetInventoryAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _inventoryRepository.GetInventoryAnalyticsAsync(startDate, endDate);
        }

        public async Task<InventoryDashboardMetrics> GetDashboardMetricsAsync()
        {
            return await _inventoryRepository.GetDashboardMetricsAsync();
        }

        public async Task<object> GetStockLevelReportAsync()
        {
            var inventory = await _inventoryRepository.GetInventoryWithMetricsAsync();
            
            return new
            {
                reportDate = DateTime.Now.ToString("yyyy-MM-dd"),
                stockLevels = inventory.GroupBy(i => i.StockStatus)
                    .Select(g => new
                    {
                        status = g.Key,
                        label = GetStockStatusLabel(g.Key),
                        count = g.Count(),
                        totalValue = g.Sum(i => i.TotalValue),
                        items = g.Select(i => new
                        {
                            id = i.Id,
                            name = i.Name,
                            category = i.Category,
                            quantity = i.Quantity,
                            threshold = i.Threshold,
                            daysOfStock = i.DaysOfStockRemaining
                        }).ToList()
                    }).ToList(),
                summary = new
                {
                    totalItems = inventory.Count(),
                    totalValue = inventory.Sum(i => i.TotalValue),
                    criticalItems = inventory.Count(i => i.StockStatus == "out_of_stock"),
                    warningItems = inventory.Count(i => i.StockStatus == "low_stock")
                }
            };
        }

        public async Task<object> GetExpirationReportAsync()
        {
            var expirationAlerts = await _inventoryRepository.GetExpirationAlertsAsync(90);
            
            return new
            {
                reportDate = DateTime.Now.ToString("yyyy-MM-dd"),
                expirationAlerts = expirationAlerts.GroupBy(a => a.AlertLevel)
                    .Select(g => new
                    {
                        alertLevel = g.Key,
                        count = g.Count(),
                        totalValue = g.Sum(a => a.Value),
                        items = g.Select(a => new
                        {
                            id = a.Id,
                            name = a.Name,
                            category = a.Category,
                            expirationDate = a.ExpirationDate?.ToString("yyyy-MM-dd"),
                            daysUntilExpiration = a.DaysUntilExpiration,
                            quantity = a.Quantity,
                            value = a.Value
                        }).OrderBy(a => a.daysUntilExpiration).ToList()
                    }).ToList(),
                summary = new
                {
                    totalExpiring = expirationAlerts.Count(),
                    totalValue = expirationAlerts.Sum(a => a.Value),
                    expired = expirationAlerts.Count(a => a.DaysUntilExpiration <= 0),
                    expiringSoon = expirationAlerts.Count(a => a.DaysUntilExpiration <= 30 && a.DaysUntilExpiration > 0)
                }
            };
        }

        public async Task<object> GetUsageReportAsync(DateTime startDate, DateTime endDate)
        {
            var movements = await _inventoryRepository.GetInventoryMovementsAsync(
                null, startDate, endDate, "out");

            var usageByItem = movements.GroupBy(m => new { m.InventoryItemId, m.ItemName })
                .Select(g => new
                {
                    itemId = g.Key.InventoryItemId,
                    itemName = g.Key.ItemName,
                    totalUsage = g.Sum(m => m.Quantity),
                    usageCount = g.Count(),
                    totalCost = g.Sum(m => m.TotalCost ?? 0),
                    averageUsage = g.Average(m => m.Quantity)
                })
                .OrderByDescending(u => u.totalUsage)
                .ToList();

            return new
            {
                period = new
                {
                    startDate = startDate.ToString("yyyy-MM-dd"),
                    endDate = endDate.ToString("yyyy-MM-dd"),
                    days = (endDate - startDate).Days
                },
                usageByItem,
                summary = new
                {
                    totalItemsUsed = usageByItem.Count,
                    totalQuantityUsed = usageByItem.Sum(u => u.totalUsage),
                    totalCostUsed = usageByItem.Sum(u => u.totalCost),
                    topUsedItems = usageByItem.Take(10).ToList()
                }
            };
        }

        public async Task<object> GetInventoryAlertsAsync()
        {
            var alerts = await _inventoryRepository.GetActiveAlertsAsync();
            
            return new
            {
                alerts = alerts.GroupBy(a => a.Severity)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                summary = new
                {
                    totalAlerts = alerts.Count(),
                    criticalAlerts = alerts.Count(a => a.Severity == "critical"),
                    warningAlerts = alerts.Count(a => a.Severity == "warning"),
                    infoAlerts = alerts.Count(a => a.Severity == "info")
                }
            };
        }

        public async Task<object> GetLowStockAlertsAsync()
        {
            var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync();
            
            return new
            {
                lowStockItems = lowStockItems.Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    category = item.Category,
                    quantity = item.Quantity,
                    threshold = item.Threshold,
                    supplier = item.Supplier,
                    urgency = CalculateUrgency(item.Quantity, item.Threshold),
                    suggestedOrderQuantity = CalculateSuggestedOrderQuantity(item.Quantity, item.Threshold)
                }).OrderBy(i => i.urgency).ToList(),
                summary = new
                {
                    totalLowStockItems = lowStockItems.Count(),
                    criticalItems = lowStockItems.Count(i => i.Quantity == 0),
                    warningItems = lowStockItems.Count(i => i.Quantity > 0 && i.Quantity <= i.Threshold)
                }
            };
        }

        public async Task<object> GetExpirationAlertsAsync(int days = 30)
        {
            var expirationAlerts = await _inventoryRepository.GetExpirationAlertsAsync(days);
            
            return new
            {
                days,
                expirationAlerts = expirationAlerts.Select(alert => new
                {
                    id = alert.Id,
                    name = alert.Name,
                    category = alert.Category,
                    expirationDate = alert.ExpirationDate?.ToString("yyyy-MM-dd"),
                    daysUntilExpiration = alert.DaysUntilExpiration,
                    quantity = alert.Quantity,
                    value = alert.Value,
                    alertLevel = alert.AlertLevel,
                    urgency = alert.DaysUntilExpiration <= 0 ? "critical" :
                             alert.DaysUntilExpiration <= 7 ? "high" :
                             alert.DaysUntilExpiration <= 30 ? "medium" : "low"
                }).OrderBy(a => a.daysUntilExpiration).ToList(),
                summary = new
                {
                    totalItems = expirationAlerts.Count(),
                    totalValue = expirationAlerts.Sum(a => a.Value),
                    expired = expirationAlerts.Count(a => a.DaysUntilExpiration <= 0),
                    expiringSoon = expirationAlerts.Count(a => a.DaysUntilExpiration <= 7 && a.DaysUntilExpiration > 0)
                }
            };
        }

        public async Task<object> GetCriticalAlertsAsync()
        {
            var criticalAlerts = await _inventoryRepository.GetActiveAlertsAsync();
            var criticalOnly = criticalAlerts.Where(a => a.Severity == "critical").ToList();
            
            return new
            {
                criticalAlerts = criticalOnly,
                summary = new
                {
                    totalCritical = criticalOnly.Count,
                    requiresImmediateAction = criticalOnly.Count(a => 
                        a.AlertType == "out_of_stock" || a.AlertType == "expired")
                },
                recommendations = GenerateCriticalRecommendations(criticalOnly)
            };
        }

        public async Task<object> GetReorderSuggestionsAsync()
        {
            var suggestions = await _inventoryRepository.GetReorderSuggestionsAsync();
            
            return new
            {
                reorderSuggestions = suggestions.OrderByDescending(s => s.Priority).ToList(),
                summary = new
                {
                    totalSuggestions = suggestions.Count(),
                    totalEstimatedCost = suggestions.Sum(s => s.EstimatedCost),
                    criticalItems = suggestions.Count(s => s.Priority == "critical"),
                    highPriorityItems = suggestions.Count(s => s.Priority == "high")
                }
            };
        }

        public async Task<object> GetInventoryForecastAsync(int itemId, int days = 30)
        {
            return await _inventoryRepository.GetInventoryForecastAsync(itemId, days);
        }

        public async Task<bool> UpdateThresholdAsync(int itemId, int newThreshold)
        {
            if (newThreshold < 0)
                throw new ArgumentException("Threshold não pode ser negativo");

            return await _inventoryRepository.UpdateThresholdAsync(itemId, newThreshold);
        }

        public async Task<object> GetInventoryValuationAsync(DateTime? date = null)
        {
            return await _inventoryRepository.GetInventoryValuationAsync(date);
        }

        public async Task<object> GetCategoryAnalysisAsync()
        {
            return await _inventoryRepository.GetCategoryMetricsAsync();
        }

        public async Task<object> GetSupplierPerformanceAsync()
        {
            return await _inventoryRepository.GetSupplierPerformanceAsync();
        }

        public async Task<object> BulkUpdateInventoryAsync(InventoryBulkAction action)
        {
            var result = await _inventoryRepository.BulkUpdateAsync(action);
            
            return new
            {
                success = result,
                action = action.Action,
                itemsAffected = action.ItemIds.Length,
                executedAt = DateTime.UtcNow
            };
        }

        public async Task<object> ExportInventoryAsync(InventoryExportRequest request)
        {
            return await _inventoryRepository.ExportInventoryAsync(request);
        }

        public async Task<object> ImportInventoryAsync(object importData)
        {
            return await _inventoryRepository.ImportInventoryAsync(importData);
        }

        public async Task<object> GetCategoriesAsync()
        {
            var categories = await _inventoryRepository.GetCategoriesAsync();
            var categoryMetrics = await _inventoryRepository.GetCategoryMetricsAsync();
            
            return new
            {
                categories = categories.ToList(),
                categoryMetrics,
                summary = new
                {
                    totalCategories = categories.Count(),
                    topCategory = categories.FirstOrDefault()
                }
            };
        }

        public async Task<object> GetSuppliersAsync()
        {
            var suppliers = await _inventoryRepository.GetSuppliersAsync();
            
            return new
            {
                suppliers = suppliers.ToList(),
                summary = new
                {
                    totalSuppliers = suppliers.Count()
                }
            };
        }

        public async Task<object> GetCategoryMetricsAsync()
        {
            return await _inventoryRepository.GetCategoryMetricsAsync();
        }

        public async Task<InventorySettings> GetInventorySettingsAsync()
        {
            return await _inventoryRepository.GetSettingsAsync();
        }

        public async Task<bool> UpdateInventorySettingsAsync(InventorySettings settings)
        {
            return await _inventoryRepository.UpdateSettingsAsync(settings);
        }

        public async Task<object> GetInventoryOptimizationAsync()
        {
            var inventory = await _inventoryRepository.GetInventoryWithMetricsAsync();
            
            return new
            {
                optimization = new
                {
                    overStockedItems = inventory.Where(i => i.Quantity > i.Threshold * 3).Count(),
                    underStockedItems = inventory.Where(i => i.Quantity <= i.Threshold).Count(),
                    deadStockItems = inventory.Where(i => i.UsageInLast30Days == 0 && i.Quantity > 0).Count(),
                    fastMovingItems = inventory.Where(i => i.TurnoverRate > 6).Count(),
                    slowMovingItems = inventory.Where(i => i.TurnoverRate < 2 && i.TurnoverRate > 0).Count()
                },
                recommendations = new[]
                {
                    "Reduzir estoque de itens com baixo giro",
                    "Aumentar estoque de itens com alto giro",
                    "Revisar fornecedores para itens de estoque morto",
                    "Implementar sistema de reposição automática"
                }
            };
        }

        public async Task<object> GetTurnoverAnalysisAsync()
        {
            var inventory = await _inventoryRepository.GetInventoryWithMetricsAsync();
            
            return new
            {
                turnoverAnalysis = inventory.Where(i => i.TurnoverRate > 0)
                    .Select(i => new
                    {
                        itemId = i.Id,
                        itemName = i.Name,
                        category = i.Category,
                        turnoverRate = i.TurnoverRate,
                        classification = ClassifyTurnover(i.TurnoverRate)
                    })
                    .OrderByDescending(i => i.turnoverRate)
                    .ToList(),
                summary = new
                {
                    averageTurnover = inventory.Where(i => i.TurnoverRate > 0).Average(i => i.TurnoverRate),
                    highTurnoverItems = inventory.Count(i => i.TurnoverRate > 6),
                    lowTurnoverItems = inventory.Count(i => i.TurnoverRate < 2 && i.TurnoverRate > 0)
                }
            };
        }

        public async Task<object> GetDeadStockAnalysisAsync()
        {
            var inventory = await _inventoryRepository.GetInventoryWithMetricsAsync();
            var deadStockItems = inventory.Where(i => i.UsageInLast30Days == 0 && i.Quantity > 0).ToList();
            
            return new
            {
                deadStockItems = deadStockItems.Select(i => new
                {
                    itemId = i.Id,
                    itemName = i.Name,
                    category = i.Category,
                    quantity = i.Quantity,
                    value = i.TotalValue,
                    lastRestocked = i.LastRestocked,
                    daysWithoutMovement = i.LastRestocked.HasValue ? 
                        (DateTime.Now - i.LastRestocked.Value).Days : 0
                }).ToList(),
                summary = new
                {
                    totalDeadStockItems = deadStockItems.Count,
                    totalDeadStockValue = deadStockItems.Sum(i => i.TotalValue),
                    percentageOfInventory = inventory.Any() ? 
                        (decimal)deadStockItems.Count / inventory.Count() * 100 : 0
                },
                recommendations = new[]
                {
                    "Considerar liquidação de itens sem movimento",
                    "Revisar necessidade de manter estes itens em estoque",
                    "Verificar se itens podem ser utilizados em outros procedimentos"
                }
            };
        }

        // Métodos auxiliares privados
        private async Task ValidateInventoryItemAsync(CreateInventoryItemDto item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Nome do item é obrigatório");

            if (string.IsNullOrWhiteSpace(item.Category))
                throw new ArgumentException("Categoria é obrigatória");

            if (item.Quantity < 0)
                throw new ArgumentException("Quantidade não pode ser negativa");

            if (item.Threshold < 0)
                throw new ArgumentException("Threshold não pode ser negativo");

            if (item.Price < 0)
                throw new ArgumentException("Preço não pode ser negativo");
        }

        private string GetStockStatusLabel(string status)
        {
            return status switch
            {
                "out_of_stock" => "Sem estoque",
                "low_stock" => "Estoque baixo",
                "warning" => "Atenção",
                "in_stock" => "Em estoque",
                _ => "Desconhecido"
            };
        }

        private string CalculateUrgency(int quantity, int threshold)
        {
            if (quantity == 0) return "critical";
            if (quantity <= threshold * 0.5) return "high";
            if (quantity <= threshold) return "medium";
            return "low";
        }

        private int CalculateSuggestedOrderQuantity(int currentQuantity, int threshold)
        {
            return Math.Max(threshold * 2 - currentQuantity, threshold);
        }

        private List<string> GenerateCriticalRecommendations(List<InventoryAlert> criticalAlerts)
        {
            var recommendations = new List<string>();
            
            if (criticalAlerts.Any(a => a.AlertType == "out_of_stock"))
                recommendations.Add("Reabastecer itens sem estoque imediatamente");
            
            if (criticalAlerts.Any(a => a.AlertType == "expired"))
                recommendations.Add("Remover itens vencidos do estoque");
            
            if (criticalAlerts.Any(a => a.AlertType == "low_stock"))
                recommendations.Add("Planejar reposição de itens com estoque baixo");
            
            return recommendations;
        }

        private string ClassifyTurnover(decimal turnoverRate)
        {
            return turnoverRate switch
            {
                > 6 => "Alto giro",
                > 2 => "Giro médio",
                > 0 => "Baixo giro",
                _ => "Sem giro"
            };
        }
    }
}