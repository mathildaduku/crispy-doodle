using AutoMapper.QueryableExtensions;
using ContentService.Data;
using ContentService.Dto.Response;
using ContentService.Helpers;
using ContentService.Models;
using ContentService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static MassTransit.ValidationResultExtensions;

namespace ContentService.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            this._context = context;
        }
        public void Add(Comment comment)
        {
            _context.Comments.Add(comment);
        }
        public void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }

        public async Task<Comment?> GetById(Guid commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<PagedResponse<List<Comment>>> GetCommentsForAPost(SearchParams searchParams, Guid postId)
        {
            var query = _context.Comments.AsQueryable();

            query = query.Include(x => x.Replies).Where(x => x.PostId == postId).OrderByDescending(x => x.CreatedAt);

            var count = await query.CountAsync();

            var result = await query.Skip((searchParams.PageNumber - 1) * searchParams.PageSize).Take(searchParams.PageSize).ToListAsync();
            var payload = new PagedResponse<List<Comment>>
            {
                results = result,
                totalCount = count,
                page = searchParams.PageNumber,
                pageSize = searchParams.PageSize,
            };
            return payload;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResponse<List<Comment>>> GetRepliesForComment(SearchParams searchParams, Guid commentId)
        {
            var query = _context.Comments.AsQueryable();

            query = query.Where(x=> x.ParentCommentId == commentId).OrderByDescending(x => x.CreatedAt);

            var count = await query.CountAsync();

            var result = await query.Skip((searchParams.PageNumber - 1) * searchParams.PageSize).Take(searchParams.PageSize).ToListAsync();
            var payload = new PagedResponse<List<Comment>>
            {
                results = result,
                totalCount = count,
                page = searchParams.PageNumber,
                pageSize = searchParams.PageSize,
            };
            return payload;
        }
    }
}
