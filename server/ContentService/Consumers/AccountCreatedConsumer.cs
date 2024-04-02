using AutoMapper;
using ContentService.Data;
using ContentService.Models;
using Contracts;
using MassTransit;

namespace SearchService.Consumers;

public class AccountCreatedConsumer : IConsumer<AccountCreated>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public AccountCreatedConsumer(IMapper mapper, AppDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    public async Task Consume(ConsumeContext<AccountCreated> context)
    {
        Console.WriteLine("--> Consuming account created: " + context.Message.Id);
        var user = _mapper.Map<User>(context.Message);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync(); 
    }
}
