using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, ILogger<SubscriptionService> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        // Subscription management
        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            try
            {
                return await _subscriptionRepository.GetAllSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as assinaturas");
                throw;
            }
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int id)
        {
            try
            {
                return await _subscriptionRepository.GetSubscriptionByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            try
            {
                subscription.CreatedAt = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;
                return await _subscriptionRepository.CreateSubscriptionAsync(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar assinatura");
                throw;
            }
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            try
            {
                subscription.UpdatedAt = DateTime.UtcNow;
                return await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar assinatura: {Id}", subscription.Id);
                throw;
            }
        }

        public async Task<bool> DeleteSubscriptionAsync(int id)
        {
            try
            {
                return await _subscriptionRepository.DeleteSubscriptionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar assinatura: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            try
            {
                return await _subscriptionRepository.GetActiveSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas ativas");
                throw;
            }
        }

        // Client subscription management
        public async Task<IEnumerable<ClientSubscription>> GetAllClientSubscriptionsAsync()
        {
            try
            {
                return await _subscriptionRepository.GetAllClientSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as assinaturas de clientes");
                throw;
            }
        }

        public async Task<ClientSubscription?> GetClientSubscriptionByIdAsync(int id)
        {
            try
            {
                return await _subscriptionRepository.GetClientSubscriptionByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura de cliente por ID: {Id}", id);
                throw;
            }
        }

        public async Task<ClientSubscription> CreateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            try
            {
                clientSubscription.CreatedAt = DateTime.UtcNow;
                clientSubscription.UpdatedAt = DateTime.UtcNow;
                clientSubscription.Status = "Active";
                
                // Calculate next payment date based on billing cycle
                var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(clientSubscription.SubscriptionId);
                if (subscription != null)
                {
                    clientSubscription.NextPaymentDate = CalculateNextPaymentDate(clientSubscription.StartDate, subscription.BillingCycle);
                }
                
                return await _subscriptionRepository.CreateClientSubscriptionAsync(clientSubscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar assinatura de cliente");
                throw;
            }
        }

        public async Task<ClientSubscription> UpdateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            try
            {
                clientSubscription.UpdatedAt = DateTime.UtcNow;
                return await _subscriptionRepository.UpdateClientSubscriptionAsync(clientSubscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar assinatura de cliente: {Id}", clientSubscription.Id);
                throw;
            }
        }

        public async Task<bool> DeleteClientSubscriptionAsync(int id)
        {
            try
            {
                return await _subscriptionRepository.DeleteClientSubscriptionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar assinatura de cliente: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ClientSubscription>> GetClientSubscriptionsByClientIdAsync(int clientId)
        {
            try
            {
                return await _subscriptionRepository.GetClientSubscriptionsByClientIdAsync(clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas do cliente: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<ClientSubscription?> GetActiveClientSubscriptionAsync(int clientId)
        {
            try
            {
                return await _subscriptionRepository.GetActiveClientSubscriptionAsync(clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinatura ativa do cliente: {ClientId}", clientId);
                throw;
            }
        }

        // Business logic methods
        public async Task<bool> ActivateSubscriptionAsync(int clientSubscriptionId)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetClientSubscriptionByIdAsync(clientSubscriptionId);
                if (subscription == null) return false;

                subscription.Status = "Active";
                subscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateClientSubscriptionAsync(subscription);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar assinatura: {Id}", clientSubscriptionId);
                throw;
            }
        }

        public async Task<bool> SuspendSubscriptionAsync(int clientSubscriptionId)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetClientSubscriptionByIdAsync(clientSubscriptionId);
                if (subscription == null) return false;

                subscription.Status = "Suspended";
                subscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateClientSubscriptionAsync(subscription);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao suspender assinatura: {Id}", clientSubscriptionId);
                throw;
            }
        }

        public async Task<bool> CancelSubscriptionAsync(int clientSubscriptionId)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetClientSubscriptionByIdAsync(clientSubscriptionId);
                if (subscription == null) return false;

                subscription.Status = "Cancelled";
                subscription.EndDate = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateClientSubscriptionAsync(subscription);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar assinatura: {Id}", clientSubscriptionId);
                throw;
            }
        }

        public async Task<bool> RenewSubscriptionAsync(int clientSubscriptionId)
        {
            try
            {
                var clientSubscription = await _subscriptionRepository.GetClientSubscriptionByIdAsync(clientSubscriptionId);
                if (clientSubscription == null) return false;

                var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(clientSubscription.SubscriptionId);
                if (subscription == null) return false;

                clientSubscription.LastPaymentDate = DateTime.UtcNow;
                clientSubscription.NextPaymentDate = CalculateNextPaymentDate(DateTime.UtcNow, subscription.BillingCycle);
                clientSubscription.Status = "Active";
                clientSubscription.UpdatedAt = DateTime.UtcNow;
                
                await _subscriptionRepository.UpdateClientSubscriptionAsync(clientSubscription);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar assinatura: {Id}", clientSubscriptionId);
                throw;
            }
        }

        public async Task<IEnumerable<ClientSubscription>> GetExpiredSubscriptionsAsync()
        {
            try
            {
                return await _subscriptionRepository.GetExpiredSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas expiradas");
                throw;
            }
        }

        public async Task<IEnumerable<ClientSubscription>> GetSubscriptionsDueForRenewalAsync(int daysAhead = 7)
        {
            try
            {
                return await _subscriptionRepository.GetSubscriptionsDueForRenewalAsync(daysAhead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar assinaturas próximas do vencimento");
                throw;
            }
        }

        private DateTime CalculateNextPaymentDate(DateTime startDate, string billingCycle)
        {
            return billingCycle.ToLower() switch
            {
                "monthly" => startDate.AddMonths(1),
                "quarterly" => startDate.AddMonths(3),
                "yearly" => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };
        }
    }
}