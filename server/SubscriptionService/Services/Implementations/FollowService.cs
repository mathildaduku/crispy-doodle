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
                //convert string ID's to Guid
                Guid followerGuid = Guid.Parse(followerId);
                Guid followeeGuid = Guid.Parse(followeeId);

                //check if the follow relationship already exists
                var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);

                if (existingFollow != null)
                {
                    //the user is already following the other user
                    throw new InvalidOperationException("You are already following this user.");
                }

                //create a new follow relationship
                var newFollow = new Follow
                {
                    FollowerId = followerGuid,
                    FolloweeId = followeeGuid
                };

                //add the new follow relationship to the db
                await _context.Follows.AddAsync(newFollow);

                //update follower's followingCount
                await _userService.UpdateFollowingCount(followerGuid, true);

                //update followee's followersCount
                await _userService.UpdateFollowersCount(followeeGuid, true);

                //save changes to the db
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while following the user.", ex);
            }
           
        }

        public async Task<PagedResult<string>> GetFolloweesAsync(string followerId, int pageNumber, int pageSize)
        {
            //convert string ID to Guid
            Guid followerGuid = Guid.Parse(followerId);

            //get total count of followees
            int totalCount = await _context.Follows.Where(f => f.FollowerId == followerGuid).CountAsync();

            //calculate the number of items to skip based on the page number and page size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            //get the follow relationships for the follower with pagination
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
            //convert string ID to Guid
            Guid followeeGuid = Guid.Parse(followeeId);

            //query the database to retrieve follow relationships for the followee with pagination.
            var followersQuery = _context.Follows.Where(f => f.FolloweeId == followeeGuid).Select(f => f.FollowerId.ToString());

            var totalCount = await followersQuery.CountAsync();
            var followers = await followersQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<string>(followers, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            Guid followerGuid = Guid.Parse(followerId);
            Guid followeeGuid = Guid.Parse(followeeId);

            //check if the follow relationship exists
            var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);
            bool isFollowing = existingFollow != null;

            return isFollowing;

        }

        public async Task UnfollowUserAsync(string followerId, string followeeId)
        {
            //convert string IDs to Guid
            Guid followerGuid = Guid.Parse(followerId);
            Guid followeeGuid = Guid.Parse(followeeId);

            //check if the user is following the other user
            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerGuid && f.FolloweeId == followeeGuid);
            if (follow == null)
            {
                throw new InvalidOperationException("You are not following this user.");
            }

            //unsubscribe if subscribed
            var isSubscribed = await _userService.IsSubscribedAsync(followerId, followeeId);
            if (isSubscribed)
            {
                var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriberUserId == followerId && s.TargetUserId == followeeId);
                if (subscription != null)
                {
                    _context.Subscriptions.Remove(subscription);
                }
            }

            //remove the follow relationship from the context
            _context.Follows.Remove(follow);

            //update follower's followingCount
            await _userService.UpdateFollowingCount(followerGuid, false);

            //update followee's followersCount
            await _userService.UpdateFollowersCount(followeeGuid, false);

            //save changes to the database
            await _context.SaveChangesAsync();
        }


    }
}

