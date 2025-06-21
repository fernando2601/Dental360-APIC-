using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        // Dashboard Financeiro
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetFinancialDashboard(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var dashboard = await _financialService.GetFinancialDashboardAsync(startDate, endDate);
            return Ok(dashboard);
        }

        // Fluxo de Caixa
        [HttpGet("cash-flow")]
        public async Task<ActionResult> GetCashFlow(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string period = "monthly")
        {
            var cashFlow = await _financialService.GetCashFlowAsync(startDate, endDate, period);
            return Ok(cashFlow);
        }

        // Transações
        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<FinancialTransaction>>> GetAllTransactions(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? type = null,
            [FromQuery] string? category = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25)
        {
            var transactions = await _financialService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("transactions/{id}")]
        public async Task<ActionResult<FinancialTransaction>> GetTransaction(int id)
        {
            var transaction = await _financialService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();
            return Ok(transaction);
        }

        [HttpPost("transactions")]
        public async Task<ActionResult<FinancialTransaction>> CreateTransaction([FromBody] FinancialTransaction transaction)
        {
            var newTransaction = await _financialService.CreateTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransaction), new { id = newTransaction.Id }, newTransaction);
        }

        [HttpPut("transactions/{id}")]
        public async Task<ActionResult<FinancialTransaction>> UpdateTransaction(int id, [FromBody] FinancialTransaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest("O ID da transação não corresponde.");
            }

            var updatedTransaction = await _financialService.UpdateTransactionAsync(transaction);
            if (updatedTransaction == null)
                return NotFound();

            return Ok(updatedTransaction);
        }

        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var result = await _financialService.DeleteTransactionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Despesas
        [HttpGet("expenses")]
        public async Task<ActionResult> GetExpenses(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? category = null)
        {
            var expenses = await _financialService.GetExpensesAsync(startDate, endDate, category);
            return Ok(expenses);
        }

        [HttpGet("expenses/categories")]
        public async Task<ActionResult> GetExpenseCategories()
        {
            var categories = await _financialService.GetExpenseCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("expenses/analysis")]
        public async Task<ActionResult> GetExpenseAnalysis(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var analysis = await _financialService.GetExpenseAnalysisAsync(startDate, endDate);
            return Ok(analysis);
        }

        // Projeções
        [HttpGet("projections")]
        public async Task<ActionResult> GetFinancialProjections(
            [FromQuery] int months = 6,
            [FromQuery] string type = "linear")
        {
            var projections = await _financialService.GetFinancialProjectionsAsync(months, type);
            return Ok(projections);
        }

        [HttpPost("projections")]
        public async Task<ActionResult> CreateProjection([FromBody] object projectionData)
        {
            var projection = await _financialService.CreateProjectionAsync(projectionData);
            return Ok(projection);
        }

        // Análises Avançadas
        [HttpGet("advanced-analysis")]
        public async Task<ActionResult> GetAdvancedAnalysis(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string analysisType = "complete")
        {
            var analysis = await _financialService.GetAdvancedAnalysisAsync(startDate, endDate, analysisType);
            return Ok(analysis);
        }

        [HttpGet("profitability")]
        public async Task<ActionResult> GetProfitabilityAnalysis(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var profitability = await _financialService.GetProfitabilityAnalysisAsync(startDate, endDate);
            return Ok(profitability);
        }

        [HttpGet("trends")]
        public async Task<ActionResult> GetFinancialTrends(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string period = "monthly")
        {
            var trends = await _financialService.GetFinancialTrendsAsync(startDate, endDate, period);
            return Ok(trends);
        }

        // Relatórios
        [HttpGet("reports/summary")]
        public async Task<ActionResult> GetFinancialSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var summary = await _financialService.GetFinancialSummaryAsync(startDate, endDate);
            return Ok(summary);
        }

        [HttpGet("reports/download")]
        public async Task<ActionResult> DownloadFinancialReport(
            [FromQuery] string format = "pdf",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string reportType = "complete")
        {
            var report = await _financialService.GenerateFinancialReportAsync(format, startDate, endDate, reportType);
            return File(report.Content, report.ContentType, report.FileName);
        }
    }
}