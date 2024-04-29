using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace NotificationService;

public class EmailService: IEmailService
{
  private readonly string _templatesFolderPath;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
 
    public EmailService(string templatesFolderPath, ILogger logger, IConfiguration configuration)
    {
        _templatesFolderPath = templatesFolderPath;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendHtmlEmailAsync(string toEmail, string subject, string templateFileName, object model)
    {
        var templateFileNameWithSuffix = templateFileName + ".html";

        var templatePath = Path.Combine(_templatesFolderPath, templateFileNameWithSuffix);

        System.Console.WriteLine(templatePath);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templateFileNameWithSuffix}");
        }

        var templateContent = await File.ReadAllTextAsync(templatePath);
        var mergedContent = MergeTemplateWithModel(templateContent, model);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Crispy Doodle", _configuration["EmailSettings:smtpUser"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mergedContent
        };

        message.Body = bodyBuilder.ToMessageBody();



        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_configuration["EmailSettings:smtpServer"], int.Parse(_configuration["EmailSettings:smtpPort"] ?? "465"), true);
            await client.AuthenticateAsync(_configuration["EmailSettings:smtpUser"], _configuration["EmailSettings:smtpPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        _logger.LogDebug($"{templateFileName} email sent to {toEmail}");
    }

    private string MergeTemplateWithModel(string templateContent, object model)
    {
        foreach (var property in model.GetType().GetProperties())
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(model)?.ToString();
            templateContent = templateContent.Replace(placeholder, value);
        }

        return templateContent;
    }
}
