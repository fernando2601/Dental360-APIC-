using System.ComponentModel.DataAnnotations;

namespace DentalSpa.Application.DTOs
{
    public class PackageCreateRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Preço original é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço original deve ser maior que zero")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "Número de sessões é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Deve haver pelo menos uma sessão")]
        public int SessionsIncluded { get; set; }

        [Required(ErrorMessage = "Dias de validade é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Validade deve ser de pelo menos um dia")]
        public int ValidityDays { get; set; }
    }

    public class PackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int SessionsIncluded { get; set; }
        public int ValidityDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool HasDiscount { get; set; }
        public decimal PricePerSession { get; set; }
    }

    public class ClientPackageCreateRequest
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Pacote é obrigatório")]
        public int PackageId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }

    public class ClientPackageDTO
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int PackageId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int SessionsUsed { get; set; }
        public string Status { get; set; } = string.Empty;
        public ClientDTO? Client { get; set; }
        public PackageDTO? Package { get; set; }
        public int RemainingSessions { get; set; }
        public bool IsExpired { get; set; }
        public bool IsCompleted { get; set; }
        public bool CanUse { get; set; }
    }
}