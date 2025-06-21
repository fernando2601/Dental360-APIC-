using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DentalSpa.Infrastructure.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IDbConnection _connection;

        public PackageRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Package>> GetAllAsync()
        {
            const string sql = "SELECT * FROM packages WHERE is_active = 1";
            return await Task.FromResult(_connection.Query<Package>(sql));
        }

        public async Task<Package?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM packages WHERE id = @Id AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirstOrDefault<Package>(sql, new { Id = id }));
        }

        public async Task<Package> CreateAsync(Package package)
        {
            const string sql = @"
                INSERT INTO packages (name, description, original_price, final_price, discount_percentage, is_active, created_at, updated_at)
                VALUES (@Name, @Description, @OriginalPrice, @FinalPrice, @DiscountPercentage, 1, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;
            
            var id = await Task.FromResult(_connection.QuerySingle<int>(sql, package));
            package.Id = id;
            return package;
        }

        public async Task<Package?> UpdateAsync(int id, Package package)
        {
            const string sql = @"
                UPDATE packages 
                SET name = @Name, description = @Description, 
                    original_price = @OriginalPrice, final_price = @FinalPrice, 
                    discount_percentage = @DiscountPercentage, updated_at = @UpdatedAt
                WHERE id = @Id AND is_active = 1";
            
            package.Id = id;
            package.UpdatedAt = DateTime.UtcNow;
            
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, package));
            return rowsAffected > 0 ? package : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE packages SET is_active = 0 WHERE id = @Id";
            var rowsAffected = await Task.FromResult(_connection.Execute(sql, new { Id = id }));
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Package>> SearchAsync(string searchTerm)
        {
            const string sql = "SELECT * FROM packages WHERE is_active = 1 AND name LIKE @SearchTerm";
            return await Task.FromResult(_connection.Query<Package>(sql, new { SearchTerm = $"%{searchTerm}%" }));
        }

        public async Task<IEnumerable<Package>> GetByCategoryAsync(string category)
        {
            const string sql = "SELECT * FROM packages WHERE category = @Category AND is_active = 1";
            return await Task.FromResult(_connection.Query<Package>(sql, new { Category = category }));
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            const string sql = "SELECT DISTINCT category FROM packages WHERE category IS NOT NULL AND category != ''";
            return await Task.FromResult(_connection.Query<string>(sql));
        }

        public async Task<int> GetCountAsync()
        {
            const string sql = "SELECT COUNT(*) FROM packages WHERE is_active = 1";
            return await Task.FromResult(_connection.QueryFirst<int>(sql));
        }

        public async Task<int> GetCountByCategoryAsync(string category)
        {
            const string sql = "SELECT COUNT(*) FROM packages WHERE category = @Category AND is_active = 1";
            return await Task.FromResult(_connection.QueryFirst<int>(sql, new { Category = category }));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string sql = "SELECT 1 FROM packages WHERE id = @Id AND is_active = 1";
            var result = await Task.FromResult(_connection.QueryFirstOrDefault<int>(sql, new { Id = id }));
            return result == 1;
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            const string sql = "SELECT 1 FROM packages WHERE name = @Name AND is_active = 1";
            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
            }
            
            var result = await Task.FromResult(_connection.QueryFirstOrDefault<int>(sql, new { Name = name, ExcludeId = excludeId }));
            return result == 1;
        }
    }
}
