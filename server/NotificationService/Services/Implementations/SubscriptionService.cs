using Microsoft.EntityFrameworkCore;
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

    public async Task DeleteSubscriptionAsync(Subscription unsubscribeData)
    {
        // Find and update subscription record for the unfollowed user
        var unfollowedUserSubscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == unsubscribeData.SubscriptionId);
        if (unfollowedUserSubscription != null)
        {
            // Remove subscription from the DbContext
            _dbContext.Remove(unfollowedUserSubscription);

        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Subscription>> GetUserSubscribers(Guid userId)
    {
        return await _dbContext.Subscriptions
                            .Where(s => s.NotificationTargetUserId == userId)
                            .ToListAsync();
    }
}
