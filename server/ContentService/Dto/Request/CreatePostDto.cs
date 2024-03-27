using System.ComponentModel.DataAnnotations;

namespace ContentService.Dto.Request
{
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CoverImageUrl { get; set; }
    }
}
