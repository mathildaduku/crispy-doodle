using ContentService.Models;

namespace ContentService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserById(Guid id);
    }
}
