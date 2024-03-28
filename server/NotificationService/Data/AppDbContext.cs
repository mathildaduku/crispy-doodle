using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using NotificationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            modelBuilder.Entity<Models.User>()
                .ToContainer("Users")
                .HasPartitionKey(u => u.UserId);

            // Configuring Subscriptions
            modelBuilder.Entity<Subscription>()
                .ToContainer("Subscriptions")
                .HasPartitionKey(s => s.SubscriptionId);

            modelBuilder.Entity<Models.User>().OwnsMany(u => u.Subscriptions);
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
