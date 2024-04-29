using ContentService.Dto.Response;
using ContentService.Helpers;
using ContentService.Models;

namespace ContentService.Services.Interfaces
{
    public interface ILikeService
    {
        public void Add(Like like);
        public void Remove(Like like);
        public Task<bool> SaveChangesAsync();
        public Task<Like> GetById(Guid likeId);
        public Task<Like> GetUserLastLikeForAPost(Guid userId, Guid postId);
        public Task<Like> GetUserLastLikeForAComment(Guid userId, Guid commentId);
        public Task<PagedResponse<List<GetLikesDto>>> GetAll(SearchParams searchParams);
    }
}
