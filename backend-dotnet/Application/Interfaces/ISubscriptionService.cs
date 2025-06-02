using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface ISubscriptionService
    {
        // Subscription management
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(int id);
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(int id);
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
        
        // Client subscription management
        Task<IEnumerable<ClientSubscription>> GetAllClientSubscriptionsAsync();
        Task<ClientSubscription?> GetClientSubscriptionByIdAsync(int id);
        Task<ClientSubscription> CreateClientSubscriptionAsync(ClientSubscription clientSubscription);
        Task<ClientSubscription> UpdateClientSubscriptionAsync(ClientSubscription clientSubscription);
        Task<bool> DeleteClientSubscriptionAsync(int id);
        Task<IEnumerable<ClientSubscription>> GetClientSubscriptionsByClientIdAsync(int clientId);
        Task<ClientSubscription?> GetActiveClientSubscriptionAsync(int clientId);
        
        // Business logic methods
        Task<bool> ActivateSubscriptionAsync(int clientSubscriptionId);
        Task<bool> SuspendSubscriptionAsync(int clientSubscriptionId);
        Task<bool> CancelSubscriptionAsync(int clientSubscriptionId);
        Task<bool> RenewSubscriptionAsync(int clientSubscriptionId);
        Task<IEnumerable<ClientSubscription>> GetExpiredSubscriptionsAsync();
        Task<IEnumerable<ClientSubscription>> GetSubscriptionsDueForRenewalAsync(int daysAhead = 7);
    }
}