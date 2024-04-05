using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class AppDbContext : DbContext
    {
        private readonly FunctionConfiguration _config;

        public AppDbContext(FunctionConfiguration config)
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                accountEndpoint: _config.CosmosAccountEndpoint,
                accountKey: _config.CosmosAccountKey,
                databaseName: _config.CosmosDatabaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring Users
            modelBuilder.Entity<User>()
                .ToContainer("Users")
                .HasPartitionKey(u => u.UserId);

            // Configuring Subscriptions
            modelBuilder.Entity<Subscription>()
                .ToContainer("Subscriptions")
                .HasPartitionKey(s => s.SubscriptionId);

            modelBuilder.Entity<User>().HasMany(u => u.Subscriptions);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
