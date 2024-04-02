using ContentService.Data;
using System.Data;

namespace AccountService.Data
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
