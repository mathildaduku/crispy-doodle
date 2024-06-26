using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService;
using NotificationService.Data;
using System.Reflection;

var assemblyLocation = Assembly.GetExecutingAssembly().Location;
var contentRootPath = Path.GetDirectoryName(assemblyLocation);

var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();

    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSingleton<FunctionConfiguration>();
        services.AddDbContext<AppDbContext>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IEmailService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<EmailService>>();
            var environment = provider.GetRequiredService<IWebHostEnvironment>();
            var templatesFolderPath = Path.Combine(environment.ContentRootPath, "Emails");
            return new EmailService(templatesFolderPath, logger, hostContext.Configuration);
        });
    })
    .Build();

try
{
    DbInitializer.InitDb(host);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while initializing the database: {ex}");
}

host.Run();
