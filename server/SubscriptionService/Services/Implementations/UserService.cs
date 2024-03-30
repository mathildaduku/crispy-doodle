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
               this._context = context;
        }
        public async Task<User?> GetUserById(Guid Id)
        {
            return await _context.Users.FindAsync(Id);
        }
    }
}
