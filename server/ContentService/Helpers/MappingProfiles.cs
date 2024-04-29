using ContentService.Models;
using AutoMapper;
using Contracts;
using ContentService.Dto.Request;
using ContentService.Dto.Response;

namespace AccountService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AccountCreated, User>();
            CreateMap<AccountUpdated, User>();
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, PostCreated>().ForMember(x => x.Author, s => s.MapFrom(d => d.UserId));
            CreateMap<Post, GetPostsDto>().ForMember(s => s.Author, s => s.MapFrom(d => d.UserId));
            CreateMap<User, UserDto>();
            CreateMap<CreateCommentDto, Comment>();
        }
    }
}
