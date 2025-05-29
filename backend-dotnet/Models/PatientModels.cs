namespace ClinicApi.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public string? RG { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // M, F, Other
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public string? Photo { get; set; }
        public string Status { get; set; } = "active"; // active, inactive, blocked
        public string? PreferredContactMethod { get; set; } // email, phone, whatsapp
        public string? Profession { get; set; }
        public string? MaritalStatus { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastVisit { get; set; }
        public int? CreatedBy { get; set; }

        // Computed properties
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        public string FormattedPhone => FormatPhone(Phone);
        public string FormattedCPF => FormatCPF(CPF);

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 10) return phone;
            return phone.Length == 11 
                ? $"({phone.Substring(0, 2)}) {phone.Substring(2, 5)}-{phone.Substring(7)}"
                : $"({phone.Substring(0, 2)}) {phone.Substring(2, 4)}-{phone.Substring(6)}";
        }

        private string FormatCPF(string? cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11) return cpf ?? "";
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";
        }
    }

    public class CreatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public string? RG { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public string? PreferredContactMethod { get; set; }
        public string? Profession { get; set; }
        public string? MaritalStatus { get; set; }
    }

    public class UpdatePatientDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CPF { get; set; }
        public string? RG { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string? PreferredContactMethod { get; set; }
        public string? Profession { get; set; }
        public string? MaritalStatus { get; set; }
    }

    public class PatientWithMetrics
    {
        public Patient Patient { get; set; } = new();
        public PatientMetrics Metrics { get; set; } = new();
    }

    public class PatientMetrics
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageSpentPerVisit { get; set; }
        public DateTime? LastVisit { get; set; }
        public DateTime? NextAppointment { get; set; }
        public int DaysSinceLastVisit { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal NoShowRate { get; set; }
        public string PatientSegment { get; set; } = string.Empty; // VIP, Regular, New, Inactive
        public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High
        public List<string> PreferredServices { get; set; } = new();
        public List<string> TreatmentHistory { get; set; } = new();
    }

    public class PatientAnalytics
    {
        public int TotalPatients { get; set; }
        public int ActivePatients { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public int NewPatientsLastMonth { get; set; }
        public decimal NewPatientGrowthRate { get; set; }
        public int InactivePatients { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal AverageAge { get; set; }
        public decimal AverageLifetimeValue { get; set; }
        public decimal AverageVisitsPerPatient { get; set; }
        public List<AgeDistribution> AgeDistribution { get; set; } = new();
        public List<GenderDistribution> GenderDistribution { get; set; } = new();
        public List<LocationDistribution> LocationDistribution { get; set; } = new();
        public List<SegmentDistribution> SegmentDistribution { get; set; } = new();
        public List<MonthlyRegistration> MonthlyRegistrations { get; set; } = new();
    }

    public class AgeDistribution
    {
        public string AgeRange { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class GenderDistribution
    {
        public string Gender { get; set; } = string.Empty;
        public string GenderLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class LocationDistribution
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class SegmentDistribution
    {
        public string Segment { get; set; } = string.Empty;
        public string SegmentLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MonthlyRegistration
    {
        public string Month { get; set; } = string.Empty;
        public string MonthLabel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal GrowthRate { get; set; }
    }

    public class PatientFilters
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Segment { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public DateTime? LastVisitFrom { get; set; }
        public DateTime? LastVisitTo { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public bool? HasAppointments { get; set; }
        public string? PreferredContactMethod { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 25;
        public string SortBy { get; set; } = "name";
        public string SortOrder { get; set; } = "asc";
    }

    public class PatientReport
    {
        public Patient Patient { get; set; } = new();
        public List<AppointmentSummary> Appointments { get; set; } = new();
        public List<TreatmentSummary> Treatments { get; set; } = new();
        public List<PaymentSummary> Payments { get; set; } = new();
        public PatientMetrics Metrics { get; set; } = new();
        public string GeneratedAt { get; set; } = string.Empty;
        public string GeneratedBy { get; set; } = string.Empty;
    }

    public class AppointmentSummary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Cost { get; set; }
        public string? Notes { get; set; }
    }

    public class TreatmentSummary
    {
        public int Id { get; set; }
        public string TreatmentName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public string? Notes { get; set; }
    }

    public class PaymentSummary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Reference { get; set; }
    }

    public class PatientSegmentation
    {
        public string Segment { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int PatientCount { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public decimal AverageValue { get; set; }
        public decimal AverageVisits { get; set; }
    }

    public class PatientCommunication
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty; // email, sms, whatsapp, call
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // sent, delivered, read, failed
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int? SentBy { get; set; }
    }

    public class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // general, medical, treatment, reminder
        public string Priority { get; set; } = string.Empty; // low, normal, high, urgent
        public bool IsPrivate { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class PatientDocument
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Category { get; set; } = string.Empty; // medical_record, exam, prescription, photo
        public string? Description { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? UploadedByName { get; set; }
    }

    public class PatientExportRequest
    {
        public string Format { get; set; } = "excel"; // excel, csv, pdf
        public PatientFilters? Filters { get; set; }
        public bool IncludeAppointments { get; set; } = false;
        public bool IncludePayments { get; set; } = false;
        public bool IncludeNotes { get; set; } = false;
        public bool IncludeDocuments { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class PatientBulkAction
    {
        public int[] PatientIds { get; set; } = Array.Empty<int>();
        public string Action { get; set; } = string.Empty; // activate, deactivate, delete, update_segment, send_communication
        public object? Parameters { get; set; }
        public string? Reason { get; set; }
    }

    public class PatientInsight
    {
        public string InsightType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal Impact { get; set; }
        public object Data { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
}