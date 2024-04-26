using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TestService;

var host = new HostBuilder()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<TestService.FunctionConfiguration>();
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IEmailService, EmailService>();

        var emailSettings = hostContext.Configuration.GetSection("EmailSettings");
        services.AddSingleton<IEmailService>(new EmailService(
            emailSettings["smtpServer"],
            int.Parse(emailSettings["smtpPort"]),
            emailSettings["smtpUser"],
            emailSettings["smtpPassword"]
        ));
    })
    .Build();

host.Run();










// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Configuration;
// using TestService;

// var host = new HostBuilder()
//     .ConfigureAppConfiguration((hostContext, config) =>
//     {
//         config.SetBasePath(Environment.CurrentDirectory)
//             .AddJsonFile("local.settings.json", true, true)
//             .AddEnvironmentVariables();
//     })
//     .ConfigureFunctionsWorkerDefaults()
//     .ConfigureServices((hostContext, services) =>
//     {
//         services.AddApplicationInsightsTelemetryWorkerService();
//         services.ConfigureFunctionsApplicationInsights();
//         services.AddSingleton<TestService.FunctionConfiguration>();
//         services.AddDbContext<AppDbContext>();
//         services.AddScoped<IEmailService, EmailService>();

//         // Retrieve email settings from configuration and register the EmailService
//         var emailSettings = hostContext.Configuration.GetSection("EmailSettings");
//         services.AddSingleton<IEmailService>(new EmailService(
//             emailSettings["smtpServer"],
//             int.Parse(emailSettings["smtpPort"]),
//             emailSettings["smtpUser"],
//             emailSettings["smtpPassword"]
//         ));
//     })
//     .Build();

// host.Run();
