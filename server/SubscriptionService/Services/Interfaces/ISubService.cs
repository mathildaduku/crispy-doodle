using SubscriptionService.Dto.Request;
using SubscriptionService.Dto.Response;

namespace SubscriptionService.Services.Interfaces
{
    public interface ISubService
    {
        Task<SubResponseDto> SubscribeAsync(string subscriberUserId, SubDto requestDto);

        Task<bool> UnsubscribeAsync(Guid subscriptionId);

        Task<List<SubResponseDto>> GetSubscriptionsForUserAsync(string subscriberUserId);

        Task<bool> IsSubscribedAsync(string subscriberUserId, string targetUserId);
    }
}
