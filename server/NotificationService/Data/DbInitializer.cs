using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Data
{
    public class DbInitializer
    {
            public static void InitDb(WebApplication app)
            {
                using var scope = app.Services.CreateScope();

                SeedData(scope.ServiceProvider.GetService<AppDbContext>());
            }

            private static void SeedData(AppDbContext context)
            {
                context.Database.EnsureCreated();

            }
    }
}
