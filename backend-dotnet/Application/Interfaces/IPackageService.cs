using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetPopularProductsAsync();
    }
} 