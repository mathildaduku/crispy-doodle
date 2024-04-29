using ContentService.Dto.Response;
using ContentService.Helpers;
using ContentService.Models;

namespace ContentService.Services.Interfaces
{
    public interface ICommentService
    {
        public void Add(Comment comment);
        public void Update(Comment comment);
        public Task<Comment?> GetById(Guid commentId);
        public Task<bool> SaveChangesAsync();
        public Task<PagedResponse<List<Comment>>> GetCommentsForAPost(SearchParams searchParams, Guid postId);
        public Task<PagedResponse<List<Comment>>> GetRepliesForComment(SearchParams searchParams, Guid commentId);
    };
}
