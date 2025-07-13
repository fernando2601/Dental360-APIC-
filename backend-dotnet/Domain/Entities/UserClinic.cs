using System;

namespace DentalSpa.Domain.Entities
{
    public class UserClinic
    {
        public int UserId { get; set; }
        public int ClinicId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
} 