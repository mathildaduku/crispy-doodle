
using SubscriptionService.Dto.Response;
using SubscriptionService.Models;

namespace SubscriptionService.Services.Interfaces
{
    public interface IFollowService
    {
        
        Task FollowUserAsync(Guid followerId, Guid followeeId);

        Task UnfollowUserAsync(Guid followerId, Guid followeeId);

        Task<bool> IsFollowingAsync(Guid followerId, Guid followeeId);

        public Task<PagedResult<string>> GetFolloweesAsync(Guid followerId, int pageNumber, int pageSize);

        public Task<PagedResult<string>> GetFollowersAsync(Guid followeeId, int pageNumber, int pageSize);
    }

}
