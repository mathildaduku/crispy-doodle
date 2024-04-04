using System.Security.Claims;

namespace SubscriptionService.Services.Interfaces
{
    public interface IUserIdentityService
    {
        string GetUserIdFromClaims(ClaimsPrincipal user);
    }
}
