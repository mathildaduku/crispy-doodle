using AutoMapper;
using ContentService.Data;
using ContentService.Models;
using Contracts;
using MassTransit;

namespace SearchService.Consumers;

public class AccountUpdatedConsumer : IConsumer<AccountUpdated>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _dbContext;

    public AccountUpdatedConsumer(IMapper mapper, AppDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    public async Task Consume(ConsumeContext<AccountUpdated> context)
    {
        Console.WriteLine("--> Consuming account updated: " + context.Message.Id);
        var user = _mapper.Map<User>(context.Message);

        _dbContext.Users.Update(user);

        await _dbContext.SaveChangesAsync();
    }
}
