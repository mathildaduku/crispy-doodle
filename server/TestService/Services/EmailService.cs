using MimeKit;
using MailKit.Net.Smtp;
using TestService;

namespace TestService;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPassword)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPassword = smtpPassword;
    }

    public void SendEmail(string from, string to, string subject, string body)
    {
        // Console.WriteLine($"Sending email from {from} to {to} with subject {subject} and body {body}");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", from));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using (var  client = new SmtpClient())        
                {
            client.Connect(_smtpServer, _smtpPort, true);
            client.Authenticate(_smtpUser, _smtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }

}
