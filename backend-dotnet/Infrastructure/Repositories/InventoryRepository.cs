using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IDbConnection _connection;

        public InventoryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<Inventory?>(null);
        }

        public async Task<Inventory> CreateAsync(Inventory inventory)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO inventory (name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at) 
                                   VALUES (@Name, @Description, @Category, @Quantity, @Unit, @MinStock, @UnitPrice, @Supplier, @Location, @BatchNumber, @ExpirationDate, @Status, @IsActive, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@Name", inventory.Name));
                cmd.Parameters.Add(CreateParameter("@Description", inventory.Description));
                cmd.Parameters.Add(CreateParameter("@Category", inventory.Category));
                cmd.Parameters.Add(CreateParameter("@Quantity", inventory.Quantity));
                cmd.Parameters.Add(CreateParameter("@Unit", inventory.Unit));
                cmd.Parameters.Add(CreateParameter("@MinStock", inventory.MinStock));
                cmd.Parameters.Add(CreateParameter("@UnitPrice", inventory.UnitPrice));
                cmd.Parameters.Add(CreateParameter("@Supplier", inventory.Supplier));
                cmd.Parameters.Add(CreateParameter("@Location", inventory.Location));
                cmd.Parameters.Add(CreateParameter("@BatchNumber", inventory.BatchNumber));
                cmd.Parameters.Add(CreateParameter("@ExpirationDate", inventory.ExpirationDate));
                cmd.Parameters.Add(CreateParameter("@Status", inventory.Status));
                cmd.Parameters.Add(CreateParameter("@IsActive", true));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                inventory.Id = id;
                inventory.CreatedAt = DateTime.Now;
                inventory.UpdatedAt = DateTime.Now;
                return await Task.FromResult(inventory);
            }
        }

        public async Task<Inventory?> UpdateAsync(int id, Inventory inventory)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE inventory SET name = @Name, description = @Description, category = @Category, quantity = @Quantity, unit = @Unit, min_stock = @MinStock, unit_price = @UnitPrice, supplier = @Supplier, location = @Location, batch_number = @BatchNumber, expiration_date = @ExpirationDate, status = @Status, is_active = @IsActive, updated_at = @UpdatedAt WHERE id = @Id AND is_active = 1";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Name", inventory.Name));
                cmd.Parameters.Add(CreateParameter("@Description", inventory.Description));
                cmd.Parameters.Add(CreateParameter("@Category", inventory.Category));
                cmd.Parameters.Add(CreateParameter("@Quantity", inventory.Quantity));
                cmd.Parameters.Add(CreateParameter("@Unit", inventory.Unit));
                cmd.Parameters.Add(CreateParameter("@MinStock", inventory.MinStock));
                cmd.Parameters.Add(CreateParameter("@UnitPrice", inventory.UnitPrice));
                cmd.Parameters.Add(CreateParameter("@Supplier", inventory.Supplier));
                cmd.Parameters.Add(CreateParameter("@Location", inventory.Location));
                cmd.Parameters.Add(CreateParameter("@BatchNumber", inventory.BatchNumber));
                cmd.Parameters.Add(CreateParameter("@ExpirationDate", inventory.ExpirationDate));
                cmd.Parameters.Add(CreateParameter("@Status", inventory.Status));
                cmd.Parameters.Add(CreateParameter("@IsActive", inventory.IsActive));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? inventory : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE inventory SET is_active = 0 WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Inventory>> SearchAsync(string searchTerm)
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE is_active = 1 AND (name LIKE @SearchTerm OR description LIKE @SearchTerm OR category LIKE @SearchTerm)";
                cmd.Parameters.Add(CreateParameter("@SearchTerm", $"%{searchTerm}%"));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE category = @Category AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Category", category));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE quantity <= min_stock AND is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<IEnumerable<Inventory>> GetExpiredAsync()
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, category, quantity, unit, min_stock, unit_price, supplier, location, batch_number, expiration_date, status, is_active, created_at, updated_at FROM inventory WHERE expiration_date IS NOT NULL AND expiration_date <= @CurrentDate AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@CurrentDate", DateTime.Now));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Category = reader.GetString(reader.GetOrdinal("category")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            Unit = reader.GetString(reader.GetOrdinal("unit")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("min_stock")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                            Supplier = reader.IsDBNull(reader.GetOrdinal("supplier")) ? null : reader.GetString(reader.GetOrdinal("supplier")),
                            Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString(reader.GetOrdinal("location")),
                            BatchNumber = reader.IsDBNull(reader.GetOrdinal("batch_number")) ? null : reader.GetString(reader.GetOrdinal("batch_number")),
                            ExpirationDate = reader.IsDBNull(reader.GetOrdinal("expiration_date")) ? null : reader.GetDateTime(reader.GetOrdinal("expiration_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<int> GetCountAsync()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE is_active = 1";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<int> GetCountByCategoryAsync(string category)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE category = @Category AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Category", category));
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<int> GetLowStockCountAsync()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE quantity <= min_stock AND is_active = 1";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count > 0);
            }
        }

        private IDbDataParameter CreateParameter(string name, object? value)
        {
            var param = _connection.CreateCommand().CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
}