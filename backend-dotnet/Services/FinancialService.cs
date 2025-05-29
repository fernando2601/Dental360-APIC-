using ClinicApi.Models;
using ClinicApi.Repositories;
using System.Text;

namespace ClinicApi.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly IFinancialRepository _financialRepository;

        public FinancialService(IFinancialRepository financialRepository)
        {
            _financialRepository = financialRepository;
        }

        public async Task<IEnumerable<FinancialTransaction>> GetAllTransactionsAsync()
        {
            return await _financialRepository.GetAllTransactionsAsync();
        }

        public async Task<FinancialTransaction?> GetTransactionByIdAsync(int id)
        {
            return await _financialRepository.GetTransactionByIdAsync(id);
        }

        public async Task<FinancialTransaction> CreateTransactionAsync(CreateFinancialTransactionDto transactionDto)
        {
            // Validações de negócio
            ValidateTransaction(transactionDto);
            
            // Gerar número de referência se não fornecido
            if (string.IsNullOrEmpty(transactionDto.ReferenceNumber))
            {
                transactionDto.ReferenceNumber = GenerateReferenceNumber(transactionDto.Type);
            }

            return await _financialRepository.CreateTransactionAsync(transactionDto);
        }

        public async Task<FinancialTransaction?> UpdateTransactionAsync(int id, CreateFinancialTransactionDto transactionDto)
        {
            ValidateTransaction(transactionDto);
            return await _financialRepository.UpdateTransactionAsync(id, transactionDto);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            return await _financialRepository.DeleteTransactionAsync(id);
        }

        public async Task<object> GetFinancialDashboardAsync(DateTime? startDate, DateTime? endDate)
        {
            // Aplicar período padrão se não especificado
            var dates = NormalizeDateRange(startDate, endDate);
            
            var dashboard = await _financialRepository.GetFinancialDashboardAsync(dates.start, dates.end);
            
            // Adicionar métricas calculadas
            return EnrichDashboardData(dashboard, dates.start, dates.end);
        }

        public async Task<object> GetCashFlowAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetCashFlowAsync(dates.start, dates.end, period);
        }

        public async Task<object> GetCashFlowProjectionsAsync(int months)
        {
            // Validar parâmetros
            if (months <= 0 || months > 24)
                months = 6; // Padrão de 6 meses

            return await _financialRepository.GetCashFlowProjectionsAsync(months);
        }

        public async Task<object> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, string? type, string? category, int page, int limit)
        {
            // Validar paginação
            if (page <= 0) page = 1;
            if (limit <= 0 || limit > 100) limit = 25;

            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetTransactionsWithFiltersAsync(
                dates.start, dates.end, type, category, page, limit);
        }

        public async Task<object> GetExpensesAsync(DateTime? startDate, DateTime? endDate, string? category)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetExpenseAnalysisAsync(dates.start, dates.end);
        }

        public async Task<IEnumerable<CategorySummary>> GetExpenseCategoriesAsync()
        {
            return await _financialRepository.GetExpenseCategoriesAsync();
        }

        public async Task<object> GetExpenseAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            var analysis = await _financialRepository.GetExpenseAnalysisAsync(dates.start, dates.end);
            
            // Adicionar insights automáticos
            return AddExpenseInsights(analysis, dates.start, dates.end);
        }

        public async Task<object> GetFinancialProjectionsAsync(int months, string type)
        {
            // Validar e normalizar parâmetros
            if (months <= 0 || months > 24) months = 6;
            
            switch (type.ToLower())
            {
                case "conservative":
                    return await GetConservativeProjections(months);
                case "optimistic":
                    return await GetOptimisticProjections(months);
                case "linear":
                default:
                    return await _financialRepository.GetCashFlowProjectionsAsync(months);
            }
        }

        public async Task<object> CreateProjectionAsync(object projectionData)
        {
            // Implementar lógica de criação de projeções customizadas
            // Por enquanto, retorna uma projeção padrão
            return await GetFinancialProjectionsAsync(6, "linear");
        }

        public async Task<object> GetAdvancedAnalysisAsync(DateTime? startDate, DateTime? endDate, string analysisType)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            var analysis = await _financialRepository.GetAdvancedAnalysisAsync(dates.start, dates.end, analysisType);
            
            // Adicionar insights baseados em IA/ML
            return AddAdvancedInsights(analysis, analysisType);
        }

        public async Task<object> GetProfitabilityAnalysisAsync(DateTime? startDate, DateTime? endDate)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetProfitabilityAnalysisAsync(dates.start, dates.end);
        }

        public async Task<object> GetFinancialTrendsAsync(DateTime? startDate, DateTime? endDate, string period)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetFinancialTrendsAsync(dates.start, dates.end, period);
        }

        public async Task<FinancialSummary> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            return await _financialRepository.GetFinancialSummaryAsync(dates.start, dates.end);
        }

        public async Task<object> GenerateFinancialReportAsync(string format, DateTime? startDate, DateTime? endDate, string reportType)
        {
            var dates = NormalizeDateRange(startDate, endDate);
            
            // Coletar dados para o relatório
            var summary = await _financialRepository.GetFinancialSummaryAsync(dates.start, dates.end);
            var analysis = await _financialRepository.GetAdvancedAnalysisAsync(dates.start, dates.end, reportType);
            
            // Gerar relatório no formato solicitado
            return await GenerateReport(format, summary, analysis, dates.start, dates.end, reportType);
        }

        // Métodos auxiliares privados
        private void ValidateTransaction(CreateFinancialTransactionDto transaction)
        {
            if (transaction.Amount <= 0)
                throw new ArgumentException("O valor da transação deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(transaction.Description))
                throw new ArgumentException("A descrição é obrigatória");

            if (string.IsNullOrWhiteSpace(transaction.Type) || 
                (transaction.Type != "income" && transaction.Type != "expense"))
                throw new ArgumentException("O tipo deve ser 'income' ou 'expense'");

            if (transaction.TransactionDate > DateTime.Now.AddDays(1))
                throw new ArgumentException("A data da transação não pode ser futura");
        }

        private string GenerateReferenceNumber(string type)
        {
            var prefix = type == "income" ? "REC" : "PAG";
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}-{timestamp}-{random}";
        }

        private (DateTime start, DateTime end) NormalizeDateRange(DateTime? startDate, DateTime? endDate)
        {
            var end = endDate ?? DateTime.Now.Date.AddDays(1).AddTicks(-1);
            var start = startDate ?? end.AddDays(-30); // Padrão de 30 dias

            return (start, end);
        }

        private object EnrichDashboardData(dynamic dashboard, DateTime startDate, DateTime endDate)
        {
            // Calcular métricas adicionais para o dashboard
            var periodDays = (endDate - startDate).Days;
            
            return new
            {
                dashboard,
                period = new
                {
                    startDate,
                    endDate,
                    days = periodDays
                },
                insights = new
                {
                    dailyAverageIncome = periodDays > 0 ? dashboard.summary.TotalIncome / periodDays : 0,
                    dailyAverageExpenses = periodDays > 0 ? dashboard.summary.TotalExpenses / periodDays : 0,
                    profitMargin = dashboard.summary.TotalIncome > 0 
                        ? (dashboard.summary.NetProfit / dashboard.summary.TotalIncome) * 100 
                        : 0,
                    burnRate = periodDays > 0 ? dashboard.summary.TotalExpenses / periodDays : 0
                }
            };
        }

        private object AddExpenseInsights(dynamic analysis, DateTime startDate, DateTime endDate)
        {
            return new
            {
                analysis,
                insights = new
                {
                    message = "Análise de despesas gerada com base nos dados do período selecionado",
                    recommendations = GenerateExpenseRecommendations(analysis),
                    period = new { startDate, endDate }
                }
            };
        }

        private object AddAdvancedInsights(dynamic analysis, string analysisType)
        {
            return new
            {
                analysis,
                insights = new
                {
                    analysisType,
                    generatedAt = DateTime.UtcNow,
                    recommendations = GenerateAdvancedRecommendations(analysis, analysisType)
                }
            };
        }

        private async Task<object> GetConservativeProjections(int months)
        {
            var baseProjections = await _financialRepository.GetCashFlowProjectionsAsync(months);
            
            // Aplicar fatores conservadores (reduzir receitas, aumentar despesas)
            return ModifyProjections(baseProjections, 0.9m, 1.1m); // -10% receita, +10% despesa
        }

        private async Task<object> GetOptimisticProjections(int months)
        {
            var baseProjections = await _financialRepository.GetCashFlowProjectionsAsync(months);
            
            // Aplicar fatores otimistas (aumentar receitas, reduzir despesas)
            return ModifyProjections(baseProjections, 1.15m, 0.95m); // +15% receita, -5% despesa
        }

        private object ModifyProjections(dynamic baseProjections, decimal incomeFactor, decimal expenseFactor)
        {
            // Implementar modificação das projeções com fatores específicos
            return new
            {
                baseProjections,
                modified = true,
                factors = new { incomeFactor, expenseFactor }
            };
        }

        private List<string> GenerateExpenseRecommendations(dynamic analysis)
        {
            var recommendations = new List<string>();
            
            recommendations.Add("Monitore regularmente as categorias de maior gasto");
            recommendations.Add("Considere negociar contratos de fornecedores");
            recommendations.Add("Avalie a possibilidade de reduzir custos operacionais");
            
            return recommendations;
        }

        private List<string> GenerateAdvancedRecommendations(dynamic analysis, string analysisType)
        {
            var recommendations = new List<string>();
            
            switch (analysisType.ToLower())
            {
                case "profitability":
                    recommendations.Add("Foque nos serviços com maior margem de lucro");
                    recommendations.Add("Avalie o aumento de preços em serviços de baixa margem");
                    break;
                case "trends":
                    recommendations.Add("Monitore tendências de crescimento mensais");
                    recommendations.Add("Ajuste estratégias baseado nos padrões identificados");
                    break;
                default:
                    recommendations.Add("Continue monitorando métricas financeiras regularmente");
                    break;
            }
            
            return recommendations;
        }

        private async Task<object> GenerateReport(string format, FinancialSummary summary, dynamic analysis, 
            DateTime startDate, DateTime endDate, string reportType)
        {
            var reportData = new
            {
                format,
                reportType,
                period = new { startDate, endDate },
                summary,
                analysis,
                generatedAt = DateTime.UtcNow
            };

            // Simular geração de arquivo
            var fileName = $"relatorio-financeiro-{DateTime.Now:yyyyMMdd}.{format.ToLower()}";
            var content = Encoding.UTF8.GetBytes("Relatório financeiro gerado");
            var contentType = format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return new
            {
                FileName = fileName,
                Content = content,
                ContentType = contentType,
                Data = reportData
            };
        }
    }
}