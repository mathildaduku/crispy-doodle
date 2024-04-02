using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Dto.Request;
using SubscriptionService.Dto.Response;
using SubscriptionService.Models;
using SubscriptionService.Services.Interfaces;

namespace SubscriptionService.Services.Implementations
{
    public class SubService : ISubService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;


        public SubService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SubResponseDto>> GetSubscriptionsForUserAsync(string subscriberUserId)
        {
            var subscriptions = await _context.Subscriptions
                .Where(s => s.SubscriberUserId == subscriberUserId)
                .ToListAsync();

            return _mapper.Map<List<SubResponseDto>>(subscriptions);
        }

        public async Task<bool> IsSubscribedAsync(string subscriberUserId, string targetUserId)
        {
            return await _context.Subscriptions
               .AnyAsync(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == targetUserId);
        }

        public async Task<SubResponseDto> SubscribeAsync(string subscriberUserId, SubDto requestDto)
        {
            // check if the subscription already exists
            var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == requestDto.TargetUserId);

            if (existingSubscription != null)
            {
                throw new InvalidOperationException("You are already subscribed to this user.");
            }


            // create sub 
            var subscription = new Subscription
            {
                SubscriberUserId = subscriberUserId,
                TargetUserId = requestDto.TargetUserId
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubResponseDto>(subscription);
        }

        public async Task<bool> UnsubscribeAsync(Guid subscriptionId)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
                return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}