using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContentService.Data;
using ContentService.Dto.Response;
using ContentService.Helpers;
using ContentService.Models;
using ContentService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public LikeService(IMapper mapper, AppDbContext context)
        {
            this._mapper = mapper;
            this._context = context;
        }
        public void Add(Like like)
        {
            _context.Likes.Add(like);
        }

        public async Task<PagedResponse<List<GetLikesDto>>> GetAll(SearchParams searchParams)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Where(like => like.User.FirstName.ToLower().Contains(searchParams.SearchTerm.ToLower()) || like.User.LastName.ToLower().Contains(searchParams.SearchTerm.ToLower()));
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var count = query.Count();

            var result = await query.Skip((searchParams.PageNumber - 1) * searchParams.PageSize).Take(searchParams.PageSize).ProjectTo<GetLikesDto>(_mapper.ConfigurationProvider).ToListAsync();
            var payload = new PagedResponse<List<GetLikesDto>>
            {
                results = result,
                totalCount = count,
                page = searchParams.PageNumber,
                pageSize = searchParams.PageSize,
            };
            return payload;
        }

        public async Task<Like?> GetById(Guid likeId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.Id == likeId);
        }

        public async Task<Like?> GetUserLastLikeForAPost(Guid userId, Guid postId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }
        public async Task<Like?> GetUserLastLikeForAComment(Guid userId, Guid commentId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);
        }



        public void Remove(Like like)
        {
            _context.Likes.Remove(like);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
