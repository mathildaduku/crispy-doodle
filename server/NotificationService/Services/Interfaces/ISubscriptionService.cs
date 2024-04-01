using NotificationService.Models;

namespace NotificationService;

public interface ISubscriptionService
{
    public Task AddSubscriptionAsync(Subscription subscription);
}
