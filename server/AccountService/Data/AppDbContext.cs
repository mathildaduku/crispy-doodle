using AccountService.Models;
using AspNetCore.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data
{
    public class AppDbContext : CosmosIdentityDbContext<User, IdentityRole, string>
    {

        public AppDbContext(DbContextOptions dbContextOptions): base(dbContextOptions) 
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
