using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class CreateFinancialTransactionDto
    {
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // Income, Expense
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        public int? ClientId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        public DateTime TransactionDate { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(200)]
        public string? Reference { get; set; }
    }

    public class UpdateFinancialTransactionDto : CreateFinancialTransactionDto
    {
        public int Id { get; set; }
    }

    public class FinancialTransactionDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FinancialSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class FinancialReportDto
    {
        public FinancialSummaryDto Summary { get; set; } = null!;
        public List<CategoryReportDto> IncomeByCategory { get; set; } = new();
        public List<CategoryReportDto> ExpensesByCategory { get; set; } = new();
        public List<PaymentMethodReportDto> PaymentMethods { get; set; } = new();
    }

    public class CategoryReportDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PaymentMethodReportDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }
}