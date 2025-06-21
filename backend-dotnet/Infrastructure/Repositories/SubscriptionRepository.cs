using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using System.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IDbConnection _connection;

        public SubscriptionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            var subscriptions = new List<Subscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, duration_days, features, is_active, created_at, updated_at FROM subscriptions WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(new Subscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            DurationDays = reader.GetInt32(reader.GetOrdinal("duration_days")),
                            Features = reader.IsDBNull(reader.GetOrdinal("features")) ? null : reader.GetString(reader.GetOrdinal("features")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(subscriptions);
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, duration_days, features, is_active, created_at, updated_at FROM subscriptions WHERE id = @Id AND is_active = 1";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Subscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            DurationDays = reader.GetInt32(reader.GetOrdinal("duration_days")),
                            Features = reader.IsDBNull(reader.GetOrdinal("features")) ? null : reader.GetString(reader.GetOrdinal("features")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<Subscription?>(null);
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO subscriptions (name, description, price, duration_days, features, is_active, created_at, updated_at) 
                                   VALUES (@Name, @Description, @Price, @DurationDays, @Features, @IsActive, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@Name", subscription.Name));
                cmd.Parameters.Add(CreateParameter("@Description", subscription.Description));
                cmd.Parameters.Add(CreateParameter("@Price", subscription.Price));
                cmd.Parameters.Add(CreateParameter("@DurationDays", subscription.DurationDays));
                cmd.Parameters.Add(CreateParameter("@Features", subscription.Features));
                cmd.Parameters.Add(CreateParameter("@IsActive", true));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                subscription.Id = id;
                subscription.CreatedAt = DateTime.Now;
                subscription.UpdatedAt = DateTime.Now;
                return await Task.FromResult(subscription);
            }
        }

        public async Task<Subscription?> UpdateAsync(int id, Subscription subscription)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE subscriptions SET name = @Name, description = @Description, price = @Price, duration_days = @DurationDays, features = @Features, is_active = @IsActive, updated_at = @UpdatedAt WHERE id = @Id AND is_active = 1";
                
                cmd.Parameters.Add(CreateParameter("@Id", id));
                cmd.Parameters.Add(CreateParameter("@Name", subscription.Name));
                cmd.Parameters.Add(CreateParameter("@Description", subscription.Description));
                cmd.Parameters.Add(CreateParameter("@Price", subscription.Price));
                cmd.Parameters.Add(CreateParameter("@DurationDays", subscription.DurationDays));
                cmd.Parameters.Add(CreateParameter("@Features", subscription.Features));
                cmd.Parameters.Add(CreateParameter("@IsActive", subscription.IsActive));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0 ? subscription : null);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE subscriptions SET is_active = 0 WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            var subscriptions = new List<Subscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, duration_days, features, is_active, created_at, updated_at FROM subscriptions WHERE is_active = 1";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(new Subscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            DurationDays = reader.GetInt32(reader.GetOrdinal("duration_days")),
                            Features = reader.IsDBNull(reader.GetOrdinal("features")) ? null : reader.GetString(reader.GetOrdinal("features")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(subscriptions);
        }

        public async Task<IEnumerable<Subscription>> SearchAsync(string searchTerm)
        {
            var subscriptions = new List<Subscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, description, price, duration_days, features, is_active, created_at, updated_at FROM subscriptions WHERE is_active = 1 AND (name LIKE @SearchTerm OR description LIKE @SearchTerm)";
                cmd.Parameters.Add(CreateParameter("@SearchTerm", $"%{searchTerm}%"));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subscriptions.Add(new Subscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("price")),
                            DurationDays = reader.GetInt32(reader.GetOrdinal("duration_days")),
                            Features = reader.IsDBNull(reader.GetOrdinal("features")) ? null : reader.GetString(reader.GetOrdinal("features")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(subscriptions);
        }

        // Métodos específicos de Subscription (usados pelo serviço)
        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            return await CreateAsync(subscription);
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            return await UpdateAsync(subscription.Id, subscription);
        }

        public async Task<bool> DeleteSubscriptionAsync(int id)
        {
            return await DeleteAsync(id);
        }

        // Métodos de ClientSubscription
        public async Task<IEnumerable<ClientSubscription>> GetAllClientSubscriptionsAsync()
        {
            var clientSubscriptions = new List<ClientSubscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE status != 'cancelled'";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientSubscriptions.Add(new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(clientSubscriptions);
        }

        public async Task<ClientSubscription?> GetClientSubscriptionByIdAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<ClientSubscription?>(null);
        }

        public async Task<ClientSubscription> CreateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO client_subscriptions (client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at) 
                                   VALUES (@ClientId, @SubscriptionId, @StartDate, @EndDate, @Status, @PaidAmount, @LastPaymentDate, @NextPaymentDate, @Notes, @CreatedAt, @UpdatedAt);
                                   SELECT CAST(SCOPE_IDENTITY() as int)";
                
                cmd.Parameters.Add(CreateParameter("@ClientId", clientSubscription.ClientId));
                cmd.Parameters.Add(CreateParameter("@SubscriptionId", clientSubscription.SubscriptionId));
                cmd.Parameters.Add(CreateParameter("@StartDate", clientSubscription.StartDate));
                cmd.Parameters.Add(CreateParameter("@EndDate", clientSubscription.EndDate));
                cmd.Parameters.Add(CreateParameter("@Status", clientSubscription.Status));
                cmd.Parameters.Add(CreateParameter("@PaidAmount", clientSubscription.PaidAmount));
                cmd.Parameters.Add(CreateParameter("@LastPaymentDate", clientSubscription.LastPaymentDate));
                cmd.Parameters.Add(CreateParameter("@NextPaymentDate", clientSubscription.NextPaymentDate));
                cmd.Parameters.Add(CreateParameter("@Notes", clientSubscription.Notes));
                cmd.Parameters.Add(CreateParameter("@CreatedAt", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                clientSubscription.Id = id;
                clientSubscription.CreatedAt = DateTime.Now;
                clientSubscription.UpdatedAt = DateTime.Now;
                return await Task.FromResult(clientSubscription);
            }
        }

        public async Task<ClientSubscription> UpdateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE client_subscriptions SET client_id = @ClientId, subscription_id = @SubscriptionId, start_date = @StartDate, end_date = @EndDate, status = @Status, paid_amount = @PaidAmount, last_payment_date = @LastPaymentDate, next_payment_date = @NextPaymentDate, notes = @Notes, updated_at = @UpdatedAt WHERE id = @Id";
                
                cmd.Parameters.Add(CreateParameter("@Id", clientSubscription.Id));
                cmd.Parameters.Add(CreateParameter("@ClientId", clientSubscription.ClientId));
                cmd.Parameters.Add(CreateParameter("@SubscriptionId", clientSubscription.SubscriptionId));
                cmd.Parameters.Add(CreateParameter("@StartDate", clientSubscription.StartDate));
                cmd.Parameters.Add(CreateParameter("@EndDate", clientSubscription.EndDate));
                cmd.Parameters.Add(CreateParameter("@Status", clientSubscription.Status));
                cmd.Parameters.Add(CreateParameter("@PaidAmount", clientSubscription.PaidAmount));
                cmd.Parameters.Add(CreateParameter("@LastPaymentDate", clientSubscription.LastPaymentDate));
                cmd.Parameters.Add(CreateParameter("@NextPaymentDate", clientSubscription.NextPaymentDate));
                cmd.Parameters.Add(CreateParameter("@Notes", clientSubscription.Notes));
                cmd.Parameters.Add(CreateParameter("@UpdatedAt", DateTime.Now));
                
                cmd.ExecuteNonQuery();
                clientSubscription.UpdatedAt = DateTime.Now;
                return await Task.FromResult(clientSubscription);
            }
        }

        public async Task<bool> DeleteClientSubscriptionAsync(int id)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE client_subscriptions SET status = 'cancelled' WHERE id = @Id";
                cmd.Parameters.Add(CreateParameter("@Id", id));
                var rows = cmd.ExecuteNonQuery();
                return await Task.FromResult(rows > 0);
            }
        }

        public async Task<IEnumerable<ClientSubscription>> GetClientSubscriptionsByClientIdAsync(int clientId)
        {
            var clientSubscriptions = new List<ClientSubscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE client_id = @ClientId AND status != 'cancelled'";
                cmd.Parameters.Add(CreateParameter("@ClientId", clientId));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientSubscriptions.Add(new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(clientSubscriptions);
        }

        public async Task<ClientSubscription?> GetActiveClientSubscriptionAsync(int clientId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE client_id = @ClientId AND status = 'active' AND (end_date IS NULL OR end_date > @Now)";
                cmd.Parameters.Add(CreateParameter("@ClientId", clientId));
                cmd.Parameters.Add(CreateParameter("@Now", DateTime.Now));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        };
                    }
                }
            }
            return await Task.FromResult<ClientSubscription?>(null);
        }

        public async Task<IEnumerable<ClientSubscription>> GetExpiredSubscriptionsAsync()
        {
            var clientSubscriptions = new List<ClientSubscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE status = 'active' AND end_date IS NOT NULL AND end_date < @Now";
                cmd.Parameters.Add(CreateParameter("@Now", DateTime.Now));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientSubscriptions.Add(new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(clientSubscriptions);
        }

        public async Task<IEnumerable<ClientSubscription>> GetSubscriptionsDueForRenewalAsync(int daysAhead = 7)
        {
            var clientSubscriptions = new List<ClientSubscription>();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, client_id, subscription_id, start_date, end_date, status, paid_amount, last_payment_date, next_payment_date, notes, created_at, updated_at FROM client_subscriptions WHERE status = 'active' AND end_date IS NOT NULL AND end_date BETWEEN @Now AND @DueDate";
                cmd.Parameters.Add(CreateParameter("@Now", DateTime.Now));
                cmd.Parameters.Add(CreateParameter("@DueDate", DateTime.Now.AddDays(daysAhead)));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientSubscriptions.Add(new ClientSubscription
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            ClientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                            SubscriptionId = reader.GetInt32(reader.GetOrdinal("subscription_id")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("start_date")),
                            EndDate = reader.IsDBNull(reader.GetOrdinal("end_date")) ? null : reader.GetDateTime(reader.GetOrdinal("end_date")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            PaidAmount = reader.GetDecimal(reader.GetOrdinal("paid_amount")),
                            LastPaymentDate = reader.IsDBNull(reader.GetOrdinal("last_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("last_payment_date")),
                            NextPaymentDate = reader.IsDBNull(reader.GetOrdinal("next_payment_date")) ? null : reader.GetDateTime(reader.GetOrdinal("next_payment_date")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString(reader.GetOrdinal("notes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                        });
                    }
                }
            }
            return await Task.FromResult(clientSubscriptions);
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