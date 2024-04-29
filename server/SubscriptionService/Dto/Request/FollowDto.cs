namespace SubscriptionService.Dto.Request
{
    public class FollowDto
    {
        public Guid FollowerId { get; set; }
        public Guid FolloweeId { get; set; }
    }
}
