namespace ContentService.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public Post Post { get; set; }
    }
}
