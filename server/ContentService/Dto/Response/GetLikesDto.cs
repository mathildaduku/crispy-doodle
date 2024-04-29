using ContentService.Models;
using System.ComponentModel.DataAnnotations;

namespace ContentService.Dto.Response
{
    public class GetLikesDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public User User { get; set; }
    }
}
