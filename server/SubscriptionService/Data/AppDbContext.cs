using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;

namespace SubscriptionService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SubscriptionService.Models.User>().ToContainer("Users").HasPartitionKey(e => e.Id).HasNoDiscriminator();
            modelBuilder.Entity<Follow>().ToContainer("Follows").HasPartitionKey(e => e.FollowerId).HasNoDiscriminator();
            modelBuilder.Entity<Subscription>().ToContainer("Subscriptions").HasPartitionKey(e => e.UserId).HasNoDiscriminator();
        }

        public DbSet<SubscriptionService.Models.User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}

