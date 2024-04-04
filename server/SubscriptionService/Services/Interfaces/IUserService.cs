using SubscriptionService.Models;

namespace SubscriptionService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserById(Guid Id);
        Task<bool> IsSubscribedAsync(string subscriberUserId, string targetUserId);
        Task UpdateFollowingCount(Guid userId, bool increment);
        Task UpdateFollowersCount(Guid userId, bool increment);
    }
}
