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

        public async Task FollowUserAsync(Guid followerId, Guid followeeId)
        {
            try
            {
                //check if the follow relationship already exists
                var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (existingFollow != null)
                {
                    //the user is already following the other user
                    throw new InvalidOperationException("You are already following this user.");
                }

                //create a new follow relationship
                var newFollow = new Follow
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                };

                //add the new follow relationship to the db
                await _context.Follows.AddAsync(newFollow);

                //update follower's followingCount
                await _userService.UpdateFollowingCount(followerId, true);

                //update followee's followersCount
                await _userService.UpdateFollowersCount(followeeId, true);

                //save changes to the db
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while following the user.", ex); //db error
            }
           
        }

        public async Task<PagedResult<string>> GetFolloweesAsync(Guid followerId, int pageNumber, int pageSize)
        {
            //get the user with the given followerId
            var user = await _context.Users.FindAsync(followerId);
            if(user == null){
                throw new InvalidOperationException("User not found.");
            }
            //the FollowingCount property of the user
            int totalCount = user.FollowingCount;

            //calculate the number of items to skip based on the page number and page size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            //get the follow relationships for the follower with pagination
            var followees = await _context.Follows
                .Where(f => f.FollowerId == followerId)
                .Select(f => f.FolloweeId.ToString())
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<string>(followees, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<string>> GetFollowersAsync(Guid followeeId, int pageNumber, int pageSize)
        {
            //get the user with the given followeeId
                 var user = await _context.Users.FindAsync(followeeId);
             if (user == null)
               {
                 throw new InvalidOperationException("User not found.");
               }
            //use the FollowersCount property of the user
               int totalCount = user.FollowersCount;

            //calculate the number of items to skip based on the page number and page size
            int itemsToSkip = (pageNumber - 1) * pageSize;

            //get the follow relationships for the followee with pagination
            var followers = await _context.Follows
                .Where(f => f.FolloweeId == followeeId)
                .Select(f => f.FollowerId.ToString())
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<string>(followers, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followeeId)
        {
            //check if the follow relationship exists
            var existingFollow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
            bool isFollowing = existingFollow != null;

            return isFollowing;

        }

        public async Task UnfollowUserAsync(Guid followerId, Guid followeeId)
        {
            //check if the user is following the other user
            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
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
            await _userService.UpdateFollowingCount(followerId, false);

            //update followee's followersCount
            await _userService.UpdateFollowersCount(followeeId, false);

            //save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}

