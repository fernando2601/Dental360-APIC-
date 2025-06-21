using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface ISubscriptionRepository
    {
        // Métodos básicos de Subscription
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<Subscription?> UpdateAsync(int id, Subscription subscription);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<Subscription>> SearchAsync(string searchTerm);
        
        // Métodos específicos de Subscription (usados pelo serviço)
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(int id);
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(int id);
        
        // Métodos de ClientSubscription
        Task<IEnumerable<ClientSubscription>> GetAllClientSubscriptionsAsync();
        Task<ClientSubscription?> GetClientSubscriptionByIdAsync(int id);
        Task<ClientSubscription> CreateClientSubscriptionAsync(ClientSubscription clientSubscription);
        Task<ClientSubscription> UpdateClientSubscriptionAsync(ClientSubscription clientSubscription);
        Task<bool> DeleteClientSubscriptionAsync(int id);
        Task<IEnumerable<ClientSubscription>> GetClientSubscriptionsByClientIdAsync(int clientId);
        Task<ClientSubscription?> GetActiveClientSubscriptionAsync(int clientId);
        Task<IEnumerable<ClientSubscription>> GetExpiredSubscriptionsAsync();
        Task<IEnumerable<ClientSubscription>> GetSubscriptionsDueForRenewalAsync(int daysAhead);
    }
}