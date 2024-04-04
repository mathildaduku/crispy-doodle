using AutoMapper;
using Contracts;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFollowService _followService;

        public SubService(AppDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint, IFollowService followService)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _followService = followService;
        }

        public async Task<List<SubResponseDto>> GetSubscriptionsForUserAsync(string subscriberUserId)
        {
            var subscriptions = await _context.Subscriptions.Where(s => s.SubscriberUserId == subscriberUserId).ToListAsync();

            return _mapper.Map<List<SubResponseDto>>(subscriptions);
        }

        public async Task<bool> IsSubscribedAsync(string subscriberUserId, string targetUserId)
        {
            var subscription = await _context.Subscriptions.Where(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == targetUserId).FirstOrDefaultAsync();

            return subscription != null;
        }

        public async Task<SubResponseDto> SubscribeAsync(string subscriberUserId, SubDto requestDto)
        {
            //check if the subscriber is following the target user
            var isFollowing = await _followService.IsFollowingAsync(subscriberUserId, requestDto.TargetUserId);
            if (!isFollowing)
            {
                throw new InvalidOperationException("You must follow the user before subscribing to them.");
            }

            //check if the subscription already exists
            var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriberUserId == subscriberUserId && s.TargetUserId == requestDto.TargetUserId);

            if (existingSubscription != null)
            {
                throw new InvalidOperationException("You are already subscribed to this user.");
            }

            //create sub 
            var subscription = new Subscription
            {
                SubscriberUserId = subscriberUserId,
                TargetUserId = requestDto.TargetUserId
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            //publish the subscription event
            await _publishEndpoint.Publish<SubscriptionCreated>(new
            {
                SubscriptionId = subscription.SubscriptionId,
                SubscriberUserId = subscriberUserId,
                TargetUserId = requestDto.TargetUserId
            });

            return _mapper.Map<SubResponseDto>(subscription);
        }

        public async Task<bool> UnsubscribeAsync(Guid subscriptionId)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
                return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            //publish the subscription deleted event
            await _publishEndpoint.Publish<SubscriptionDeleted>(new
            {
                SubscriptionId = subscriptionId,
                SubscriberUserId = subscription.SubscriberUserId,
                TargetUserId = subscription.TargetUserId
            });

            return true;
        }
    }
}