
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TestService;

public class AppDbContext : DbContext

{
    // stores confiuration automatically injected into at runtime
    private readonly FunctionConfiguration _config;

    public DbSet<Notification> Notifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, FunctionConfiguration config) : base(options)
    {
        _config = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>().ToContainer("Notifications")
        .HasNoDiscriminator().HasPartitionKey("UserId");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCosmos(
            _config.CosmosAccountEndpoint,
            _config.CosmosAccountKey,
            _config.CosmosDatabaseName
        );
    }
    


}
