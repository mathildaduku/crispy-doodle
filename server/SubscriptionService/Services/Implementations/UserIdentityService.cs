using SubscriptionService.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionService.Services.Implementations
{
    public class UserIdentityService : IUserIdentityService
{
    public Guid GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (!Guid.TryParse(userId, out Guid guid))
        {
            throw new InvalidOperationException("Invalid user ID.");
        }
        return guid;
    }
}
}
