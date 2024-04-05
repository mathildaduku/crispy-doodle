using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NotificationService.Data
{
    public class DbInitializer
    {
        public static void InitDb(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<AppDbContext>();

            if (context != null)
            {
                SeedData(context);
            }
        }

        private static void SeedData(AppDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
