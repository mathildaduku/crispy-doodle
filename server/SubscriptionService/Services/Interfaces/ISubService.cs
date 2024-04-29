using SubscriptionService.Dto.Request;
using SubscriptionService.Dto.Response;

namespace SubscriptionService.Services.Interfaces
{
    public interface ISubService
    {
        Task<SubResponseDto> SubscribeAsync(Guid subscriberUserId, SubDto requestDto);

        Task<bool> UnsubscribeAsync(Guid subscriptionId);

        Task<List<SubResponseDto>> GetSubscriptionsForUserAsync(Guid subscriberUserId);

        Task<bool> IsSubscribedAsync(Guid subscriberUserId, Guid targetUserId);
    }
}
