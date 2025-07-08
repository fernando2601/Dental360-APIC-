namespace DentalSpa.Application.DTOs
{
    public class FinancialTransactionResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "completed";
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 