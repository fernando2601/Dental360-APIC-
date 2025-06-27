using System.Data;
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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, original_price, sessions_included, validity_days, is_active, created_at FROM products WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            OriginalPrice = reader.GetDecimal(reader.GetOrdinal("original_price")),
                            SessionsIncluded = reader.GetInt32(reader.GetOrdinal("sessions_included")),
                            ValidityDays = reader.GetInt32(reader.GetOrdinal("validity_days")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(products);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, original_price, sessions_included, validity_days, is_active, created_at FROM products WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            OriginalPrice = reader.GetDecimal(reader.GetOrdinal("original_price")),
                            SessionsIncluded = reader.GetInt32(reader.GetOrdinal("sessions_included")),
                            ValidityDays = reader.GetInt32(reader.GetOrdinal("validity_days")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<Product?>(null);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO products (name, description, price, original_price, sessions_included, validity_days, is_active, created_at) VALUES (@Name, @Description, @Price, @OriginalPrice, @SessionsIncluded, @ValidityDays, @IsActive, @CreatedAt); SELECT LASTVAL();";
                
                var nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@Name";
                nameParam.Value = product.Name;
                cmd.Parameters.Add(nameParam);

                var descParam = cmd.CreateParameter();
                descParam.ParameterName = "@Description";
                descParam.Value = product.Description ?? (object)DBNull.Value;
                cmd.Parameters.Add(descParam);

                var priceParam = cmd.CreateParameter();
                priceParam.ParameterName = "@Price";
                priceParam.Value = product.Price;
                cmd.Parameters.Add(priceParam);

                var origPriceParam = cmd.CreateParameter();
                origPriceParam.ParameterName = "@OriginalPrice";
                origPriceParam.Value = product.OriginalPrice;
                cmd.Parameters.Add(origPriceParam);

                var sessionsParam = cmd.CreateParameter();
                sessionsParam.ParameterName = "@SessionsIncluded";
                sessionsParam.Value = product.SessionsIncluded;
                cmd.Parameters.Add(sessionsParam);

                var validityParam = cmd.CreateParameter();
                validityParam.ParameterName = "@ValidityDays";
                validityParam.Value = product.ValidityDays;
                cmd.Parameters.Add(validityParam);

                var activeParam = cmd.CreateParameter();
                activeParam.ParameterName = "@IsActive";
                activeParam.Value = product.IsActive;
                cmd.Parameters.Add(activeParam);

                var createdParam = cmd.CreateParameter();
                createdParam.ParameterName = "@CreatedAt";
                createdParam.Value = product.CreatedAt;
                cmd.Parameters.Add(createdParam);

                var id = Convert.ToInt32(cmd.ExecuteScalar());
                product.Id = id;
                return await Task.FromResult(product);
            }
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE products SET name = @Name, description = @Description, price = @Price, original_price = @OriginalPrice, sessions_included = @SessionsIncluded, validity_days = @ValidityDays, is_active = @IsActive WHERE id = @Id";
                
                var idParam = cmd.CreateParameter();
                idParam.ParameterName = "@Id";
                idParam.Value = id;
                cmd.Parameters.Add(idParam);

                var nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@Name";
                nameParam.Value = product.Name;
                cmd.Parameters.Add(nameParam);

                var descParam = cmd.CreateParameter();
                descParam.ParameterName = "@Description";
                descParam.Value = product.Description ?? (object)DBNull.Value;
                cmd.Parameters.Add(descParam);

                var priceParam = cmd.CreateParameter();
                priceParam.ParameterName = "@Price";
                priceParam.Value = product.Price;
                cmd.Parameters.Add(priceParam);

                var origPriceParam = cmd.CreateParameter();
                origPriceParam.ParameterName = "@OriginalPrice";
                origPriceParam.Value = product.OriginalPrice;
                cmd.Parameters.Add(origPriceParam);

                var sessionsParam = cmd.CreateParameter();
                sessionsParam.ParameterName = "@SessionsIncluded";
                sessionsParam.Value = product.SessionsIncluded;
                cmd.Parameters.Add(sessionsParam);

                var validityParam = cmd.CreateParameter();
                validityParam.ParameterName = "@ValidityDays";
                validityParam.Value = product.ValidityDays;
                cmd.Parameters.Add(validityParam);

                var activeParam = cmd.CreateParameter();
                activeParam.ParameterName = "@IsActive";
                activeParam.Value = product.IsActive;
                cmd.Parameters.Add(activeParam);

                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? product : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE products SET is_active = 0 WHERE id = @Id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            var products = new List<Product>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, original_price, sessions_included, validity_days, is_active, created_at FROM products WHERE is_active = 1 AND (name ILIKE @SearchTerm OR description ILIKE @SearchTerm)";
                var param = cmd.CreateParameter();
                param.ParameterName = "@SearchTerm";
                param.Value = $"%{searchTerm}%";
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            OriginalPrice = reader.GetDecimal(reader.GetOrdinal("original_price")),
                            SessionsIncluded = reader.GetInt32(reader.GetOrdinal("sessions_included")),
                            ValidityDays = reader.GetInt32(reader.GetOrdinal("validity_days")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(products);
        }

        public async Task<int> GetCountAsync()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM products WHERE is_active = 1";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<int> GetCountByCategoryAsync(string category)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM products WHERE category = @Category AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Category";
                param.Value = category;
                cmd.Parameters.Add(param);
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT 1 FROM products WHERE id = @Id AND is_active = 1";
                var param = cmd.CreateParameter();
                param.ParameterName = "@Id";
                param.Value = id;
                cmd.Parameters.Add(param);
                using (var reader = cmd.ExecuteReader())
                {
                    return await Task.FromResult(reader.Read());
                }
            }
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            using (var cmd = _connection.CreateCommand())
            {
                if (excludeId.HasValue)
                {
                    cmd.CommandText = "SELECT 1 FROM products WHERE name = @Name AND id != @ExcludeId AND is_active = 1";
                    var excludeParam = cmd.CreateParameter();
                    excludeParam.ParameterName = "@ExcludeId";
                    excludeParam.Value = excludeId.Value;
                    cmd.Parameters.Add(excludeParam);
                }
                else
                {
                    cmd.CommandText = "SELECT 1 FROM products WHERE name = @Name AND is_active = 1";
                }

                var nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@Name";
                nameParam.Value = name;
                cmd.Parameters.Add(nameParam);

                using (var reader = cmd.ExecuteReader())
                {
                    return await Task.FromResult(reader.Read());
                }
            }
        }
    }
} 