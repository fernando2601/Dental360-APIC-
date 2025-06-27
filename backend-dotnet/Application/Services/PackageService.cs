using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                return await _productRepository.CreateAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            try
            {
                product.Id = id; // Ensure the product has the correct ID
                return await _productRepository.UpdateAsync(id, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                return await _productRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                return await _productRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active products");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetPopularProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular products");
                throw;
            }
        }
    }
} 