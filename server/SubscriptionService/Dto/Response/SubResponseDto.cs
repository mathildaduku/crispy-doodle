namespace SubscriptionService.Dto.Response
{
    public class SubResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public string SubscriberUserId { get; set; } // User who subscribes
        public string TargetUserId { get; set; } // User whose posts are being subscribed to
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
