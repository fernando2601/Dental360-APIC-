namespace ClinicApi.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string? Cpf { get; set; }
        public string? Rg { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string? Profession { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? HealthPlan { get; set; }
        public string? HealthPlanNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PatientProfile
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public int Age { get; set; }
        public string AgeGroup { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? HealthPlan { get; set; }
        public PatientStatistics Statistics { get; set; } = new();
        public List<PatientAppointment> RecentAppointments { get; set; } = new();
        public List<PatientTreatment> ActiveTreatments { get; set; } = new();
        public PatientFinancialSummary FinancialSummary { get; set; } = new();
    }

    public class PatientStatistics
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public DateTime? LastVisit { get; set; }
        public DateTime? NextAppointment { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal TotalSpent { get; set; }
        public int DaysAsPatient { get; set; }
        public string PatientType { get; set; } = string.Empty; // new, regular, vip, inactive
    }

    public class PatientAppointment
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusLabel { get; set; } = string.Empty;
        public decimal? Cost { get; set; }
        public string? Notes { get; set; }
        public string Room { get; set; } = string.Empty;
    }

    public class PatientTreatment
    {
        public int Id { get; set; }
        public string TreatmentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
    }

    public class PatientFinancialSummary
    {
        public decimal TotalSpent { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal LastPayment { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public int PendingInvoices { get; set; }
        public decimal AverageSpendingPerVisit { get; set; }
    }

    public class CreatePatientDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string? Cpf { get; set; }
        public string? Rg { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string? Profession { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string? HealthPlan { get; set; }
        public string? HealthPlanNumber { get; set; }
    }

    public class PatientMedicalHistory
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Category { get; set; } = string.Empty; // medical, dental, allergies, medications
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Severity { get; set; } = string.Empty; // low, medium, high, critical
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PatientDocument
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DocumentType { get; set; } = string.Empty; // contract, consent, exam, photo
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = false;
    }

    public class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // treatment, behavior, preference, warning
        public string Priority { get; set; } = string.Empty; // low, medium, high
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsPrivate { get; set; } = false;
    }

    public class PatientSearchResult
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? HealthPlan { get; set; }
        public DateTime? LastVisit { get; set; }
        public DateTime? NextAppointment { get; set; }
        public string PatientStatus { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int TotalAppointments { get; set; }
    }

    public class PatientAnalytics
    {
        public int TotalPatients { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public int ActivePatients { get; set; }
        public int InactivePatients { get; set; }
        public decimal AverageAge { get; set; }
        public List<GenderDistribution> GenderDistribution { get; set; } = new();
        public List<AgeGroupDistribution> AgeDistribution { get; set; } = new();
        public List<CityDistribution> CityDistribution { get; set; } = new();
        public List<HealthPlanDistribution> HealthPlanDistribution { get; set; } = new();
        public List<PatientTypeDistribution> PatientTypeDistribution { get; set; } = new();
        public PatientRetentionMetrics RetentionMetrics { get; set; } = new();
    }

    public class GenderDistribution
    {
        public string Gender { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class AgeGroupDistribution
    {
        public string AgeGroup { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Range { get; set; } = string.Empty;
    }

    public class CityDistribution
    {
        public string City { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class HealthPlanDistribution
    {
        public string HealthPlan { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PatientTypeDistribution
    {
        public string PatientType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class PatientRetentionMetrics
    {
        public decimal RetentionRate { get; set; }
        public decimal ChurnRate { get; set; }
        public decimal AverageLifetimeValue { get; set; }
        public int AverageVisitsPerYear { get; set; }
        public decimal ReactivationRate { get; set; }
        public int NewPatientAcquisition { get; set; }
    }

    public class PatientCommunication
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty; // email, sms, whatsapp, call
        public string Direction { get; set; } = string.Empty; // inbound, outbound
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // sent, delivered, read, failed
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int SentBy { get; set; }
        public string SentByName { get; set; } = string.Empty;
    }

    public class PatientSegment
    {
        public string SegmentName { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
        public int PatientCount { get; set; }
        public decimal AverageValue { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<PatientSearchResult> Patients { get; set; } = new();
    }

    public class PatientExportRequest
    {
        public string Format { get; set; } = string.Empty; // excel, csv, pdf
        public string[]? Fields { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string[]? Statuses { get; set; }
        public string[]? Cities { get; set; }
        public string[]? HealthPlans { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class PatientBulkAction
    {
        public int[] PatientIds { get; set; } = Array.Empty<int>();
        public string Action { get; set; } = string.Empty; // activate, deactivate, update_plan, send_message
        public object? Parameters { get; set; }
    }

    public class PatientDashboardMetrics
    {
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int NewPatientsToday { get; set; }
        public int NewPatientsThisWeek { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public decimal PatientGrowthRate { get; set; }
        public decimal AveragePatientValue { get; set; }
        public List<PatientTrendData> GrowthTrend { get; set; } = new();
        public List<PatientAppointment> TodayAppointments { get; set; } = new();
        public List<PatientBirthday> TodayBirthdays { get; set; } = new();
    }

    public class PatientTrendData
    {
        public DateTime Date { get; set; }
        public int NewPatients { get; set; }
        public int TotalPatients { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    public class PatientBirthday
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public bool MessageSent { get; set; } = false;
    }
}