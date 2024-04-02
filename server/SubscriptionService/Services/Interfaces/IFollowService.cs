
using SubscriptionService.Dto.Response;
using SubscriptionService.Models;

namespace SubscriptionService.Services.Interfaces
{
    public interface IFollowService
    {
        
        Task FollowUserAsync(string followerId, string followeeId);

        Task UnfollowUserAsync(string followerId, string followeeId);

        Task<bool> IsFollowingAsync(string followerId, string followeeId);

        public Task<PagedResult<string>> GetFolloweesAsync(string followerId, int pageNumber, int pageSize);

        public Task<PagedResult<string>> GetFollowersAsync(string followeeId, int pageNumber, int pageSize);
    }

}
