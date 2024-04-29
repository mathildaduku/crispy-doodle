using ContentService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(e => e.Id).HasNoDiscriminator();
            modelBuilder.Entity<Post>().ToContainer("Posts").HasPartitionKey(e => e.Id).HasNoDiscriminator();
            modelBuilder.Entity<Follow>().ToContainer("Follows").HasPartitionKey(e => e.FollowingId).HasNoDiscriminator();

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
    }
}
