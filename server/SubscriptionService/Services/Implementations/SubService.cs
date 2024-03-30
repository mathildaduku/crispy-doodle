using AutoMapper;
using SubscriptionService.Data;
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
        public async void Subscribe(Guid userId, Guid postId)
        {
            var subscription = new Subscription
            {
                UserId = userId,
                PostId = postId
            };

            _context.Subscriptions.AddAsync(subscription);
            _context.SaveChanges();
        }

        public async void Unsubscribe(Guid userId, Guid postId)
        {
            var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == userId && s.PostId == postId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                _context.SaveChangesAsync();
            }
        }
    }
}