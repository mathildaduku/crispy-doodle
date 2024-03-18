using AccountService.Dto.Request;
using AccountService.Dto.Response;
using AccountService.Models;
using AutoMapper;

namespace AccountService.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
        }
    }
}
