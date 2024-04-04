using SubscriptionService.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionService.Services.Implementations
{
    public class UserIdentityService : IUserIdentityService
    {
        public string GetUserIdFromClaims(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
