using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using NotificationService.Data;
using NotificationService.Models;
using MailKit.Net.Smtp;

namespace NotificationService
{
    public class NewPostFunction
    {
        private readonly ILogger<NewPostFunction> _logger;
        private readonly AppDbContext _dbContext;

        public NewPostFunction(ILogger<NewPostFunction> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(NewPostFunction))]
        public async Task Run(
            [ServiceBusTrigger("mytopic", "mysubscription", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a Subscription object
                var subscription = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(message.Body.ToArray()));

                // Fetch all subscriptions for the specified target user
                var subscriptions = await _dbContext.Subscriptions
                    .Where(s => s.NotificationTargetUserId == subscription.NotificationTargetUserId)
                    .ToListAsync();

                foreach (var subscriberUser in subscriptions)
                {
                    // Fetch the associated user for the subscription
                    var subscriber = await _dbContext.Users.FindAsync(subscription.SubscriberUserId);
                    if (subscriber == null)
                    {
                        _logger.LogError($"Subscriber not found for user ID: {subscription.SubscriberUserId}");
                        continue;
                    }

                    // Send notification to the subscriber
                    await SendNotification(subscriber, newPostData);

                    _logger.LogInformation($"Notification sent to {subscriber.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notifications: {ex.Message}");
            }
        }

        private async Task SendNotification(Subscription subscriber, ServiceBusReceivedMessage messagee)
        {
            // Deserialize the Service Bus message body to a Subscription object
            var subscription = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(messagee.Body.ToArray()));

            try
            {
                // Create email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Mathilda", "mathildaduku@gmail.com"));
                message.To.Add(new MailboxAddress(subscriber.SubscriberUserId.email));
                message.Subject = "New Post Notification";
                message.Body = new TextPart("plain")
                {
                    Text = $"Hi {subscriber.Name},\n\nA new post with ID {newPostData.PostId} has been published.\n\nRegards,\nYour Application"
                };

                // Connect to SMTP server and send email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.example.com", 587, false);
                    await client.AuthenticateAsync("your-email@example.com", "your-email-password");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Notification sent to {subscriber.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to {subscriber.Email}: {ex.Message}");
            }
        }

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
        }
    }
}
