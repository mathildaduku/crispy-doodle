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
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostService(AppDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public void Add(Post post)
        {
            _context.Add(post);
        }

        public async Task<PagedResponse<List<GetPostsDto>>> GetAll(PostSearchParams searchParams, Guid? userId)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = query.Where(quiz => quiz.Title.ToLower().Contains(searchParams.SearchTerm.ToLower()) || quiz.Description.ToLower().Contains(searchParams.SearchTerm.ToLower()));
            }

            //query = searchParams?.Status switch
            //{
            //    PostFilters.explore => query.Where(x => x. > DateTime.UtcNow && !x.IsCompleted),
            //    PostFilters.following => query.Where(x => x.EndTime <= DateTime.UtcNow || x.IsCompleted),
            //    _ => query
            //};

            query = query.OrderByDescending(x => x.CreatedAt);
          


            var count = query.Count();

            var result = await query.Skip((searchParams.PageNumber - 1) * searchParams.PageSize).Take(searchParams.PageSize).ProjectTo<GetPostsDto>(_mapper.ConfigurationProvider).ToListAsync();
            var payload = new PagedResponse<List<GetPostsDto>>
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

        public void MarkAsModified(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
        }

        public Task<Post> GetById(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}

