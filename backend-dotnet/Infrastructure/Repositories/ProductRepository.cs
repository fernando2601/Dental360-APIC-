using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;
        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public Task<Product?> GetByIdAsync(int id)
        {
            // Implemente a consulta ADO.NET aqui
            return Task.FromResult<Product?>(null);
        }
        public Task<IEnumerable<Product>> GetAllAsync()
        {
            // Implemente a consulta ADO.NET aqui
            return Task.FromResult<IEnumerable<Product>>(new List<Product>());
        }
        public Task<Product> CreateAsync(Product product)
        {
            // Implemente o insert ADO.NET aqui
            return Task.FromResult(product);
        }
        public Task<Product?> UpdateAsync(int id, Product product)
        {
            // Implemente o update ADO.NET aqui
            return Task.FromResult<Product?>(product);
        }
        public Task<bool> DeleteAsync(int id)
        {
            // Implemente o delete ADO.NET aqui
            return Task.FromResult(true);
        }
    }
} 