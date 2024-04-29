using ContentService.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace ContentService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Follow>()
        .HasOne(f => f.Follower)
        .WithMany()
        .HasForeignKey(f => f.FollowerId)
        .OnDelete(DeleteBehavior.Restrict); // This will prevent cascade delete

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany()
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict); // This will prevent cascade delete

            modelBuilder.Entity<Like>()
                .HasOne(l=> l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // This will prevent cascade delete
            //modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(e => e.Id).HasNoDiscriminator();
            //modelBuilder.Entity<Post>().ToContainer("Posts").HasPartitionKey(e => e.Id).HasNoDiscriminator().HasOne(p => p.User);
            //modelBuilder.Entity<Follow>().ToContainer("Follows").HasPartitionKey(e => e.FollowingId).HasNoDiscriminator();

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
