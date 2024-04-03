using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddUserAsync(User newUser)
    {
        // Add the user to the  DbContext
        _dbContext.Users.Add(newUser);

        // Save changes in the DbContext to Cosmos DB
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        // Remove the user from the DbContext
        _dbContext.Users.Remove(user);

        // Save changes in the DbContext to Cosmos DB
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetUserAsync(Guid userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }
}
