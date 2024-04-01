using NotificationService.Models;

namespace NotificationService;

public class PostService : IPostService
{
    public Task<User> GetUserByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
