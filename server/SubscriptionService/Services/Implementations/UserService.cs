using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Services.Interfaces;

namespace SubscriptionService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
               _context = context;
        }
      
        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateFollowersCount(Guid userId, bool increment)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (increment)
                    user.FollowersCount++;
                else if (user.FollowersCount > 0)
                    user.FollowersCount--;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateFollowingCount(Guid userId, bool increment)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (increment)
                    user.FollowingCount++;
                else if (user.FollowingCount > 0)
                    user.FollowingCount--;

                await _context.SaveChangesAsync();
            }
        }
    }
}
