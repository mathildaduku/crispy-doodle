using AutoMapper;
using Contracts;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Helpers
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<AccountCreated, User>().ForMember(x=> x.UserId, s => s.MapFrom(d => d.Id));
            CreateMap<AccountDeleted, User>().ForMember(x => x.UserId, s => s.MapFrom(d => d.Id));
            // CreateMap<SubscriptionCreated, Subscription>().ForMember(x => x.SubscriptionId, s => s.MapFrom(d => d.Id));
            // CreateMap<SubscriptionDeleted, Subscription>().ForMember(x => x.SubscriptionId, s => s.MapFrom(d => d.Id));
        }
    }
}
