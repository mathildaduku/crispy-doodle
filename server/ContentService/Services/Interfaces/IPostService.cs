using ContentService.Dto.Response;
using ContentService.Helpers;
using ContentService.Models;

namespace ContentService.Services.Interfaces
{
    public interface IPostService
    {
        public void Add(Post post);
        public void Update(Post post);
        public void MarkAsModified(Post post);
        public Task<bool> SaveChangesAsync();
        public Task<Post> GetById(Guid postId);
        public Task<GetPostsDto> GetByIdForAUser(Guid postId, Guid? userId);
        public Task<PagedResponse<List<GetPostsDto>>> GetAll(PostSearchParams searchParams, Guid? userId);
    }
}
