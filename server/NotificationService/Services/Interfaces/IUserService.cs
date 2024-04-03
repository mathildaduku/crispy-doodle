using NotificationService.Models;

namespace NotificationService;

public interface IUserService
{
    public Task AddUserAsync(User user);
    public Task DeleteUserAsync(User user);
    public Task UpdateUserAsync(User user);
    public Task<User?> GetUserAsync(Guid userId);
}
