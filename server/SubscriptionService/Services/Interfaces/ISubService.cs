namespace SubscriptionService.Services.Interfaces
{
    public interface ISubService
    {
        public void Subscribe(Guid postId, Guid userId);
        public void Unsubscribe(Guid postId, Guid userId);
    }
}
