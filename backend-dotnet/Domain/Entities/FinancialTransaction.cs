using System;

namespace DentalSpa.Domain.Entities
{
    public class FinancialTransaction
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // income, expense
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty; // cash, card, pix, etc
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Client? Client { get; set; }
        public Appointment? Appointment { get; set; }

        public bool IsIncome() => Type == "income";
        public bool IsExpense() => Type == "expense";
        public bool IsToday() => Date.Date == DateTime.Today;
        public bool IsThisMonth() => Date.Month == DateTime.Now.Month && Date.Year == DateTime.Now.Year;
    }
}