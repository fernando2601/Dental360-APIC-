using Microsoft.EntityFrameworkCore;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Infrastructure.Data;

namespace DentalSpa.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly DentalSpaDbContext _context;

        public SubscriptionRepository(DentalSpaDbContext context)
        {
            _context = context;
        }

        // Subscription CRUD
        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.ClientSubscriptions)
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.ClientSubscriptions)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<bool> DeleteSubscriptionAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null) return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        // Client Subscription CRUD
        public async Task<IEnumerable<ClientSubscription>> GetAllClientSubscriptionsAsync()
        {
            return await _context.ClientSubscriptions
                .Include(cs => cs.Client)
                .Include(cs => cs.Subscription)
                .ToListAsync();
        }

        public async Task<ClientSubscription?> GetClientSubscriptionByIdAsync(int id)
        {
            return await _context.ClientSubscriptions
                .Include(cs => cs.Client)
                .Include(cs => cs.Subscription)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<ClientSubscription> CreateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            _context.ClientSubscriptions.Add(clientSubscription);
            await _context.SaveChangesAsync();
            return clientSubscription;
        }

        public async Task<ClientSubscription> UpdateClientSubscriptionAsync(ClientSubscription clientSubscription)
        {
            _context.Entry(clientSubscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return clientSubscription;
        }

        public async Task<bool> DeleteClientSubscriptionAsync(int id)
        {
            var clientSubscription = await _context.ClientSubscriptions.FindAsync(id);
            if (clientSubscription == null) return false;

            _context.ClientSubscriptions.Remove(clientSubscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ClientSubscription>> GetClientSubscriptionsByClientIdAsync(int clientId)
        {
            return await _context.ClientSubscriptions
                .Include(cs => cs.Subscription)
                .Where(cs => cs.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<ClientSubscription?> GetActiveClientSubscriptionAsync(int clientId)
        {
            return await _context.ClientSubscriptions
                .Include(cs => cs.Subscription)
                .FirstOrDefaultAsync(cs => cs.ClientId == clientId && cs.Status == "Active");
        }

        public async Task<IEnumerable<ClientSubscription>> GetExpiredSubscriptionsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.ClientSubscriptions
                .Include(cs => cs.Client)
                .Include(cs => cs.Subscription)
                .Where(cs => cs.EndDate.HasValue && cs.EndDate.Value.Date < today && cs.Status == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<ClientSubscription>> GetSubscriptionsDueForRenewalAsync(int daysAhead)
        {
            var futureDate = DateTime.UtcNow.Date.AddDays(daysAhead);
            return await _context.ClientSubscriptions
                .Include(cs => cs.Client)
                .Include(cs => cs.Subscription)
                .Where(cs => cs.NextPaymentDate.HasValue && 
                           cs.NextPaymentDate.Value.Date <= futureDate &&
                           cs.Status == "Active")
                .ToListAsync();
        }
    }
}