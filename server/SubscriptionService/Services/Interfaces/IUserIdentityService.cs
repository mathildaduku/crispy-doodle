﻿using System.Security.Claims;

namespace SubscriptionService.Services.Interfaces
{
    public interface IUserIdentityService
    {
        Guid GetUserIdFromClaims(ClaimsPrincipal user);
    }
}
