using AutoMapper;
using Contracts;
using MassTransit;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Consumers
{
    public class AccountDeletedConsumer : IConsumer<AccountDeleted>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public AccountDeletedConsumer(IMapper mapper, AppDbContext appDbContext)
        {
            _mapper = mapper;
            _dbContext = appDbContext;
        }
        public async Task Consume(ConsumeContext<AccountDeleted> context)
        {
            var user = _mapper.Map<User>(context.Message);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
