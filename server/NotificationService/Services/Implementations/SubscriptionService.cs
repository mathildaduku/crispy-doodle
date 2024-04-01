using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService;

public class SubscriptionService : ISubscriptionService

{
        private readonly AppDbContext _dbContext;

    public SubscriptionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddSubscriptionAsync(Subscription subscription)
    {
        // Add the new subscription to the DbContext
        _dbContext.Subscriptions.Add(subscription);

        // Save changes in the DbContext to Cosmos DB
        await _dbContext.SaveChangesAsync();
    }
}
