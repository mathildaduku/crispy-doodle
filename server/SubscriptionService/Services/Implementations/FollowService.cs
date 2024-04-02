using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Dto.Response;
using SubscriptionService.Models;
using SubscriptionService.Services.Interfaces;

namespace SubscriptionService.Services.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public FollowService(AppDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task FollowUserAsync(string followerId, string followeeId)
        {
            try
            {
                // Convert string IDs to Guid
                Guid followerGuid = Guid.Parse(followerId);
                Guid followeeGuid = Guid.Parse(followeeId);

                // Check if the follow relationship already exists
                var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);

                if (existingFollow != null)
                {
                    // The user is already following the other user
                    throw new InvalidOperationException("You are already following this user.");
                }

                // Create a new follow relationship
                var newFollow = new Follow
                {
                    FollowerId = followerGuid,
                    FolloweeId = followeeGuid
                };

                // Add the new follow relationship to the db
                await _context.Follows.AddAsync(newFollow);

                // Update follower's followingCount
                await _userService.UpdateFollowingCount(followerGuid, true);

                // Update followee's followersCount
                await _userService.UpdateFollowersCount(followeeGuid, true);

                // Save changes to the db
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while following the user.", ex);
            }
           
        }

        public async Task<PagedResult<string>> GetFolloweesAsync(string followerId, int pageNumber, int pageSize)
        {
            // Convert string ID to Guid
            Guid followerGuid = Guid.Parse(followerId);

            // Get total count of followees
            int totalCount = await _context.Follows.Where(f => f.FollowerId == followerGuid).CountAsync();

            // Calculate the number of items to skip based on the page number and page size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            // Get the follow relationships for the follower with pagination
            var followees = await _context.Follows
                .Where(f => f.FollowerId == followerGuid)
                .Select(f => f.FolloweeId.ToString())
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<string>(followees, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<string>> GetFollowersAsync(string followeeId, int pageNumber, int pageSize)
        {
            // Convert string ID to Guid
            Guid followeeGuid = Guid.Parse(followeeId);

            // Query the database to retrieve follow relationships for the followee with pagination.
            var followersQuery = _context.Follows.Where(f => f.FolloweeId == followeeGuid).Select(f => f.FollowerId.ToString());

            var totalCount = await followersQuery.CountAsync();
            var followers = await followersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<string>(followers, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            Guid followerGuid = Guid.Parse(followerId);
            Guid followeeGuid = Guid.Parse(followeeId);

            // Check if the follow relationship exists
            var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);
            bool isFollowing = existingFollow != null;

            return isFollowing;

        }

        public async Task UnfollowUserAsync(string followerId, string followeeId)
        {
            // Convert string IDs to Guid
            Guid followerGuid = Guid.Parse(followerId);
            Guid followeeGuid = Guid.Parse(followeeId);

            // Find the follow relationship
            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);

            if (follow == null)
            {
                // The user is not following the other user
                throw new InvalidOperationException("You are not following this user.");
            }

            // Remove the follow relationship from the context
            _context.Follows.Remove(follow);

            // Update follower's followingCount
            await _userService.UpdateFollowingCount(followerGuid, false);

            // Update followee's followersCount
            await _userService.UpdateFollowersCount(followeeGuid, false);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}

