using System.ComponentModel.DataAnnotations;

namespace ContentService.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set;} = DateTime.UtcNow;
        public List<Like> Likes { get; set;}
    }
}
