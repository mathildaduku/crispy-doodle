namespace SubscriptionService.Dto.Response
{
    public class FollowResponseDto
    {
        public Guid FollowId { get; set; } //  Id of the Follow entity/relationship
        public UserDto Follower { get; set; }
        public UserDto Followee { get; set; }
    }
}
