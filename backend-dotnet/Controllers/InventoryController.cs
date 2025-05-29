using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // CRUD Básico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAllInventory()
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            var item = await _inventoryService.GetInventoryItemByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryItem>> CreateInventoryItem(CreateInventoryItemDto itemDto)
        {
            try
            {
                var item = await _inventoryService.CreateInventoryItemAsync(itemDto);
                return CreatedAtAction(nameof(GetInventoryItem), new { id = item.Id }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryItem>> UpdateInventoryItem(int id, CreateInventoryItemDto itemDto)
        {
            try
            {
                var item = await _inventoryService.UpdateInventoryItemAsync(id, itemDto);
                if (item == null)
                    return NotFound();

                return Ok(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var result = await _inventoryService.DeleteInventoryItemAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Advanced Inventory Management
        [HttpGet("with-metrics")]
        public async Task<ActionResult> GetInventoryWithMetrics()
        {
            var result = await _inventoryService.GetInventoryWithMetricsAsync();
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> GetInventoryWithFilters(
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] bool? lowStock = null,
            [FromQuery] bool? expiringSoon = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25)
        {
            var result = await _inventoryService.GetInventoryWithFiltersAsync(
                category, status, search, lowStock, expiringSoon, page, limit);
            return Ok(result);
        }

        // Stock Management
        [HttpPost("{id}/adjust-stock")]
        public async Task<ActionResult> AdjustStock(int id, [FromBody] object request)
        {
            try
            {
                // Parse request seria implementado aqui
                var result = await _inventoryService.AdjustStockAsync(id, 10, "Ajuste manual", 1);
                return Ok(new { success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/receive-stock")]
        public async Task<ActionResult> ReceiveStock(int id, [FromBody] object request)
        {
            try
            {
                // Parse request seria implementado aqui
                var result = await _inventoryService.ReceiveStockAsync(id, 50, 25.00m, 1);
                return Ok(new { success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/consume-stock")]
        public async Task<ActionResult> ConsumeStock(int id, [FromBody] object request)
        {
            try
            {
                // Parse request seria implementado aqui
                var result = await _inventoryService.ConsumeStockAsync(id, 5, 1, "Procedimento 123");
                return Ok(new { success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/stock-level")]
        public async Task<ActionResult> SetStockLevel(int id, [FromBody] object request)
        {
            try
            {
                // Parse request seria implementado aqui
                var result = await _inventoryService.SetStockLevelAsync(id, 100, "Contagem de estoque", 1);
                return Ok(new { success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Transactions & Movements
        [HttpGet("movements")]
        public async Task<ActionResult> GetInventoryMovements(
            [FromQuery] int? itemId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? transactionType = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 50)
        {
            var result = await _inventoryService.GetInventoryMovementsAsync(
                itemId, startDate, endDate, transactionType, page, limit);
            return Ok(result);
        }

        [HttpGet("{id}/transaction-history")]
        public async Task<ActionResult> GetItemTransactionHistory(int id)
        {
            var result = await _inventoryService.GetItemTransactionHistoryAsync(id);
            return Ok(result);
        }

        // Analytics & Reports
        [HttpGet("analytics")]
        public async Task<ActionResult> GetInventoryAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _inventoryService.GetInventoryAnalyticsAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("dashboard-metrics")]
        public async Task<ActionResult> GetDashboardMetrics()
        {
            var result = await _inventoryService.GetDashboardMetricsAsync();
            return Ok(result);
        }

        [HttpGet("stock-level-report")]
        public async Task<ActionResult> GetStockLevelReport()
        {
            var result = await _inventoryService.GetStockLevelReportAsync();
            return Ok(result);
        }

        [HttpGet("expiration-report")]
        public async Task<ActionResult> GetExpirationReport()
        {
            var result = await _inventoryService.GetExpirationReportAsync();
            return Ok(result);
        }

        [HttpGet("usage-report")]
        public async Task<ActionResult> GetUsageReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _inventoryService.GetUsageReportAsync(startDate, endDate);
            return Ok(result);
        }

        // Alerts & Monitoring
        [HttpGet("alerts")]
        public async Task<ActionResult> GetInventoryAlerts()
        {
            var result = await _inventoryService.GetInventoryAlertsAsync();
            return Ok(result);
        }

        [HttpGet("alerts/low-stock")]
        public async Task<ActionResult> GetLowStockAlerts()
        {
            var result = await _inventoryService.GetLowStockAlertsAsync();
            return Ok(result);
        }

        [HttpGet("alerts/expiration")]
        public async Task<ActionResult> GetExpirationAlerts([FromQuery] int days = 30)
        {
            var result = await _inventoryService.GetExpirationAlertsAsync(days);
            return Ok(result);
        }

        [HttpGet("alerts/critical")]
        public async Task<ActionResult> GetCriticalAlerts()
        {
            var result = await _inventoryService.GetCriticalAlertsAsync();
            return Ok(result);
        }

        // Reorder Management
        [HttpGet("reorder-suggestions")]
        public async Task<ActionResult> GetReorderSuggestions()
        {
            var result = await _inventoryService.GetReorderSuggestionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}/forecast")]
        public async Task<ActionResult> GetInventoryForecast(int id, [FromQuery] int days = 30)
        {
            var result = await _inventoryService.GetInventoryForecastAsync(id, days);
            return Ok(result);
        }

        [HttpPut("{id}/threshold")]
        public async Task<ActionResult> UpdateThreshold(int id, [FromBody] object request)
        {
            try
            {
                // Parse request seria implementado aqui
                var result = await _inventoryService.UpdateThresholdAsync(id, 15);
                return Ok(new { success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Valuation & Auditing
        [HttpGet("valuation")]
        public async Task<ActionResult> GetInventoryValuation([FromQuery] DateTime? date = null)
        {
            var result = await _inventoryService.GetInventoryValuationAsync(date);
            return Ok(result);
        }

        [HttpGet("category-analysis")]
        public async Task<ActionResult> GetCategoryAnalysis()
        {
            var result = await _inventoryService.GetCategoryAnalysisAsync();
            return Ok(result);
        }

        [HttpGet("supplier-performance")]
        public async Task<ActionResult> GetSupplierPerformance()
        {
            var result = await _inventoryService.GetSupplierPerformanceAsync();
            return Ok(result);
        }

        // Bulk Operations
        [HttpPost("bulk-update")]
        public async Task<ActionResult> BulkUpdateInventory([FromBody] InventoryBulkAction action)
        {
            var result = await _inventoryService.BulkUpdateInventoryAsync(action);
            return Ok(result);
        }

        [HttpPost("export")]
        public async Task<ActionResult> ExportInventory([FromBody] InventoryExportRequest request)
        {
            var result = await _inventoryService.ExportInventoryAsync(request);
            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportInventory([FromBody] object importData)
        {
            var result = await _inventoryService.ImportInventoryAsync(importData);
            return Ok(result);
        }

        // Categories & Suppliers
        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories()
        {
            var result = await _inventoryService.GetCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("suppliers")]
        public async Task<ActionResult> GetSuppliers()
        {
            var result = await _inventoryService.GetSuppliersAsync();
            return Ok(result);
        }

        [HttpGet("category-metrics")]
        public async Task<ActionResult> GetCategoryMetrics()
        {
            var result = await _inventoryService.GetCategoryMetricsAsync();
            return Ok(result);
        }

        // Settings & Configuration
        [HttpGet("settings")]
        public async Task<ActionResult> GetInventorySettings()
        {
            var result = await _inventoryService.GetInventorySettingsAsync();
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult> UpdateInventorySettings([FromBody] InventorySettings settings)
        {
            var result = await _inventoryService.UpdateInventorySettingsAsync(settings);
            return Ok(new { success = result });
        }

        // Advanced Features
        [HttpGet("optimization")]
        public async Task<ActionResult> GetInventoryOptimization()
        {
            var result = await _inventoryService.GetInventoryOptimizationAsync();
            return Ok(result);
        }

        [HttpGet("turnover-analysis")]
        public async Task<ActionResult> GetTurnoverAnalysis()
        {
            var result = await _inventoryService.GetTurnoverAnalysisAsync();
            return Ok(result);
        }

        [HttpGet("dead-stock-analysis")]
        public async Task<ActionResult> GetDeadStockAnalysis()
        {
            var result = await _inventoryService.GetDeadStockAnalysisAsync();
            return Ok(result);
        }
    }
}