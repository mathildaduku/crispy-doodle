using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models
{
    public class Follow
    {
        [Key]
        public Guid FollowId { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FolloweeId { get; set; }
    }
}
