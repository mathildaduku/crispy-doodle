using NotificationService.Models;

namespace NotificationService;

public interface IPostService
{
public Task<User> GetUserByIdAsync(string userId)
}
