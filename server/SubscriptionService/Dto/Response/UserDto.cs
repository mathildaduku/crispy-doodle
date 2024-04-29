namespace SubscriptionService.Dto.Response
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }

    }
}
