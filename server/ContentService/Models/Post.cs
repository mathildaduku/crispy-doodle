using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set;} = DateTime.UtcNow;
        public List<Like> Likes { get; set;} = new List<Like>();
        public User User { get; set; }
    }
}
