using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class FinancialTransactionCreateRequest
    {
        [Required(ErrorMessage = "Tipo é obrigatório")]
        public string Type { get; set; } = string.Empty; // income, expense

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }

        [Required(ErrorMessage = "Método de pagamento é obrigatório")]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class FinancialTransactionDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ClientDTO? Client { get; set; }
        public AppointmentDTO? Appointment { get; set; }
        public bool IsIncome { get; set; }
        public bool IsExpense { get; set; }
        public bool IsToday { get; set; }
        public bool IsThisMonth { get; set; }
    }

    public class FinancialReportRequest
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string? Type { get; set; }
        public string? Category { get; set; }
    }

    public class FinancialSummaryDTO
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit { get; set; }
        public int TransactionCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}