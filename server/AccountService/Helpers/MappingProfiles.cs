using AccountService.Dto.Request;
using AccountService.Dto.Response;
using AccountService.Models;
using AutoMapper;
using Contracts;

namespace AccountService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, AccountCreated>();
            CreateMap<User, AccountUpdated>();
            CreateMap<User, AccountDeleted>();
        }
    }
}
