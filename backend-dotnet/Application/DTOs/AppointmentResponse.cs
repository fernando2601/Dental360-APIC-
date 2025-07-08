namespace DentalSpa.Application.DTOs
{
    public class AppointmentResponse
    {
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "scheduled";
        public string? Notes { get; set; }
        public string? Room { get; set; }
        public decimal Price { get; set; }
        public string? PaymentStatus { get; set; } = "pending";
        public string? PaymentMethod { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public int? ParentAppointmentId { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ClientFeedback { get; set; }
        public int? Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 