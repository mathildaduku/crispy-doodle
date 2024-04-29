using AutoMapper;
using Contracts;
using SubscriptionService.Dto.Request;
using SubscriptionService.Dto.Response;
using SubscriptionService.Models;

namespace SubscriptionService.Helpers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<SubDto, Subscription>();
            CreateMap<Subscription, SubResponseDto>();
            CreateMap<FollowDto, Follow>();
            CreateMap<Follow, FollowResponseDto>();
            CreateMap<AccountCreated, User>();
            CreateMap<AccountDeleted, User>();
        }
    }
}
