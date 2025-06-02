using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface ISubscriptionRepository
    {
        // Subscription CRUD
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(int id);
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(int id);
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
        
        // Client Subscription CRUD
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