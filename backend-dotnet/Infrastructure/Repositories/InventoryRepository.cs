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
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE status = 'active'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
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
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE id = @Id AND status = 'active'";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
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
                cmd.CommandText = @"INSERT INTO inventory (product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at) 
                                   VALUES (@ProductId, @Quantity, @MinQuantity, @Status, @ClinicId, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@ProductId", inventory.ProductId));
                cmd.Parameters.Add(CreateParameter("@Quantity", inventory.Quantity));
                cmd.Parameters.Add(CreateParameter("@MinQuantity", inventory.MinQuantity));
                cmd.Parameters.Add(CreateParameter("@Status", inventory.Status));
                cmd.Parameters.Add(CreateParameter("@ClinicId", inventory.ClinicId));
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
                cmd.CommandText = @"UPDATE inventory SET product_id = @ProductId, quantity = @Quantity, min_quantity = @MinQuantity, status = @Status, clinic_id = @ClinicId, updated_at = @UpdatedAt WHERE id = @Id AND status = 'active'";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@ProductId", inventory.ProductId));
                cmd.Parameters.Add(CreateParameter("@Quantity", inventory.Quantity));
                cmd.Parameters.Add(CreateParameter("@MinQuantity", inventory.MinQuantity));
                cmd.Parameters.Add(CreateParameter("@Status", inventory.Status));
                cmd.Parameters.Add(CreateParameter("@ClinicId", inventory.ClinicId));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? inventory : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE inventory SET status = 'inactive' WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE status = 'active'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
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
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE quantity <= min_quantity AND status = 'active'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
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
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE status = 'active'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
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
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE status = 'active'";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<int> GetCountByCategoryAsync(string category)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE status = 'active'";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<int> GetLowStockCountAsync()
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE quantity <= min_quantity AND status = 'active'";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM inventory WHERE id = @Id AND status = 'active'";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return await Task.FromResult(count > 0);
            }
        }

        public async Task<IEnumerable<Inventory>> GetByProductIdAsync(int productId)
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE product_id = @ProductId AND status = 'active'";
                cmd.Parameters.Add(CreateParameter("@ProductId", productId));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
        }

        public async Task<IEnumerable<Inventory>> GetByClinicIdAsync(int clinicId)
        {
            var inventories = new List<Inventory>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, product_id, quantity, min_quantity, status, clinic_id, created_at, updated_at FROM inventory WHERE clinic_id = @ClinicId AND status = 'active'";
                cmd.Parameters.Add(CreateParameter("@ClinicId", clinicId));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            MinQuantity = reader.GetInt32(reader.GetOrdinal("min_quantity")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            ClinicId = reader.GetInt32(reader.GetOrdinal("clinic_id")),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(inventories);
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