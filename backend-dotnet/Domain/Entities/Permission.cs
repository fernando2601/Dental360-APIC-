namespace DentalSpa.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // SUPER_ADM, ADM
        public string? Description { get; set; }
    }
} 