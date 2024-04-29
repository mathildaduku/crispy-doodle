namespace SubscriptionService.Dto.Request
{
    public class SubDto
    {
        public Guid SubscriberUserId { get; set; }
        public Guid TargetUserId { get; set; } // user whose posts are being subscribed to

    }
}
