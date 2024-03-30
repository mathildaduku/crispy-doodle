using SubscriptionService.Models;

namespace SubscriptionService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> GetUserById(Guid Id);
    }
}
