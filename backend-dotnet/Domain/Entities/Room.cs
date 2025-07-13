using System;

namespace DentalSpa.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 