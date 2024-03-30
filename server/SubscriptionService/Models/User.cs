using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
    }
}
