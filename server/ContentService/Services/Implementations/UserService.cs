using ContentService.Data;
using ContentService.Models;
using ContentService.Services.Interfaces;

namespace ContentService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<User?> GetUserById(Guid id)
        {
          return await  _context.Users.FindAsync(id);     
        }
    }
}
