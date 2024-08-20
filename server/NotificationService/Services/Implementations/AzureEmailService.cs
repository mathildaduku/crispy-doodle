using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NotificationService.Services.Implementations
{
    public class AzureEmailService : IEmailService
    {
        private readonly string _templatesFolderPath;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public AzureEmailService(string templatesFolderPath, ILogger logger, IConfiguration configuration)
        {
            _templatesFolderPath = templatesFolderPath;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendHtmlEmailAsync(string toEmail, string subject, string templateFileName, object model)
        {
            var mergedContent = await PrepareEmailContent(templateFileName, model);
            await SendEmail(toEmail, subject, mergedContent);
            _logger.LogDebug($"{templateFileName} email sent to {toEmail}");

        }

        private async Task<string> PrepareEmailContent(string templateFileName, object model)
        {
            // Append ".html" to the template file name
            var templateFileNameWithSuffix = $"{templateFileName}.html";
            // Combine the folder path with the file name
            var templatePath = Path.Combine(_templatesFolderPath, templateFileNameWithSuffix);

            // Check if the template file exists
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templateFileNameWithSuffix}");
            }

            // Read the template content
            var templateContent = await File.ReadAllTextAsync(templatePath);

            // Merge the template with the model
            foreach (var property in model.GetType().GetProperties())
            {
                var placeholder = $"{{{{{property.Name}}}}}";
                var value = property.GetValue(model)?.ToString();
                templateContent = templateContent.Replace(placeholder, value);
            }

            return templateContent;
        }

        private async Task SendEmail(string toEmail, string subject, string mergedContent)
        {
            // Get the connection string
            string connectionString = _configuration["COMMUNICATION_SERVICES_CONNECTION_STRING"];

            // Create the email content
            var emailContent = new EmailContent(subject) { Html = mergedContent };
            // Create the email recipients list
            var emailRecipients = new EmailRecipients(new List<EmailAddress> { new EmailAddress(toEmail) });
            // Create the email message
            var message = new EmailMessage(senderAddress: "DoNotReply@73337412-f599-480d-81b3-8ff570132897.azurecomm.net", emailRecipients, emailContent);
            // Create the email client
            var client = new EmailClient(connectionString);
            try
            {
                // Send the email
                await client.SendAsync(Azure.WaitUntil.Completed, message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to send email", ex);
            }
        }

    }
}
    

        
