namespace DentalSpa.Domain.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } // em minutos
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
    }
}