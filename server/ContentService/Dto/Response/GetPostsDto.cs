namespace ContentService.Dto.Response
{
    public class GetPostsDto
    {
        public Guid Id { get; set; }
        public Guid Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
        public UserDto User { get; set; }
        public int LikeCount { get; set; }
    }
}
