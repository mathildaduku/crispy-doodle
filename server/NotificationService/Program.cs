using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NotificationService.Interfaces.IEmailService;


            var host = new HostBuilder()
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((hostContext, s) =>
                {
                    var emailSettings = hostContext.Configuration.GetSection("EmailSettings");
                    s.AddSingleton<IEmailService>(new EmailService(
                        emailSettings["smtpServer"],
                        int.Parse(emailSettings["smtpPort"]),
                        emailSettings["smtpUser"],
                        emailSettings["smtpPassword"]
                    ));
                })
                .Build();

            host.Run();
        }
    }
}