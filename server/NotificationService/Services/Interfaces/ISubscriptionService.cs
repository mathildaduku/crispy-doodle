using NotificationService.Models;

namespace NotificationService;

public interface ISubscriptionService
{
    public Task AddSubscriptionAsync(Subscription subscription);
    public Task DeleteSubscriptionAsync(Subscription subscription);
   public Task<List<Subscription>> GetUserSubscribers(Guid userId);
}
