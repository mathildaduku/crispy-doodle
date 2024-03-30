using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Services.Interfaces;

namespace SubscriptionService.Services.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FollowService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task FollowUserAsync(string followerId, string followeeId)
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

            // Save changes to the db
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetFolloweesAsync(string followerId)
        {
            // Convert string ID to Guid
            Guid followerGuid = Guid.Parse(followerId);

            // Get the follow relationships for the follower
            var followees = await _context.Follows
                .Where(f => f.FollowerId == followerGuid)
                .Select(f => f.FolloweeId.ToString())
                .ToListAsync();

            return followees;
        }

        public async Task<List<string>> GetFollowersAsync(string followeeId)
        {
            // Convert string ID to Guid
            Guid followeeGuid = Guid.Parse(followeeId);

            // Get the follow relationships for the followee
            var followers = await _context.Follows
                .Where(f => f.FolloweeId == followeeGuid)
                .Select(f => f.FollowerId.ToString())
                .ToListAsync();

            return followers;
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            Guid followerGuid = Guid.Parse(followerId);
            Guid followeeGuid = Guid.Parse(followeeId);

            // Check if the follow relationship exists
           // bool isFollowing = await _context.Follows.AnyAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);
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

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

    }
}

