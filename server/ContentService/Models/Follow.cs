using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Follow
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public User Follower { get; set; }
        public User Following { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
