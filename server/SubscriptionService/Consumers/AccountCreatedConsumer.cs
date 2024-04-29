using AutoMapper;
using Contracts;
using MassTransit;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Consumers
{
    public class AccountCreatedConsumer : IConsumer<AccountCreated>
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        public AccountCreatedConsumer(IMapper mapper, AppDbContext appDbContext)
        {
            _mapper = mapper;
            _dbContext = appDbContext;
        }
        public async Task Consume(ConsumeContext<AccountCreated> context)
        {
            var user = _mapper.Map<User>(context.Message);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
