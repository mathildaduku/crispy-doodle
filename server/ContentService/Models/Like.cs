using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Like
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
