using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Follow
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
    }
}
