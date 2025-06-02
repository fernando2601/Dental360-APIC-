namespace ClinicApi.Models
{
    public class FinancialTransaction
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // "income" ou "expense"
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "dinheiro", "cartao", "pix", "transferencia"
        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "completed"; // "pending", "completed", "cancelled"
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateFinancialTransactionDto
    {
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "completed";
    }

    public class FinancialSummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalTransactions { get; set; }
        public List<CategorySummary> IncomeByCategory { get; set; } = new();
        public List<CategorySummary> ExpensesByCategory { get; set; } = new();
    }

    public class CategorySummary
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}