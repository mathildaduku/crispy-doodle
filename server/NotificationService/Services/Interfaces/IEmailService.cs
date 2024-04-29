using MimeKit;

namespace NotificationService;

public interface IEmailService
{
Task SendHtmlEmailAsync(string toEmail, string subject, string templateFileName, object model);
}
