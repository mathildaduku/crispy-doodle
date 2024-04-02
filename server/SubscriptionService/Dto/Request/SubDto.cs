namespace SubscriptionService.Dto.Request
{
    public class SubDto
    {
        public string SubscriberUserId { get; set; }
        public string TargetUserId { get; set; } // user whose posts are being subscribed to

    }
}
