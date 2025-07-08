using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialTransactionResponse>>> GetAll()
        {
            var transactions = await _financialService.GetAllAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialTransactionResponse>> GetById(int id)
        {
            var transaction = await _financialService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound();
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<FinancialTransactionResponse>> Create([FromBody] FinancialTransaction transaction)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newTransaction = await _financialService.CreateAsync(transaction);
            var response = new FinancialTransactionResponse
            {
                Type = newTransaction.Type,
                Category = newTransaction.Category,
                Amount = newTransaction.Amount,
                Description = newTransaction.Description,
                Date = newTransaction.Date,
                PaymentMethod = newTransaction.PaymentMethod,
                ClientId = newTransaction.ClientId,
                AppointmentId = newTransaction.AppointmentId,
                ReferenceNumber = newTransaction.ReferenceNumber,
                Status = newTransaction.Status,
                CreatedAt = newTransaction.CreatedAt,
                UpdatedAt = newTransaction.UpdatedAt
            };
            return CreatedAtAction(nameof(GetById), null, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FinancialTransactionResponse>> Update(int id, [FromBody] FinancialTransaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest("O ID da transação não corresponde.");
            }

            var updatedTransaction = await _financialService.UpdateAsync(id, transaction);
            if (updatedTransaction == null)
                return NotFound();

            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _financialService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FinancialTransactionResponse>>> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var transactions = await _financialService.SearchAsync(term);
            return Ok(transactions);
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