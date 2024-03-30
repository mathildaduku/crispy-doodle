namespace SubscriptionService.Services.Interfaces
{
    public interface IFollowService
    {
        
        Task FollowUserAsync(string followerId, string followeeId);

        Task UnfollowUserAsync(string followerId, string followeeId);

        Task<bool> IsFollowingAsync(string followerId, string followeeId);

        Task<List<string>> GetFolloweesAsync(string followerId);

        Task<List<string>> GetFollowersAsync(string followeeId);
    }

}
