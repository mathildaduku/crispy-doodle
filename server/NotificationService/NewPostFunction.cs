using System;
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
            [ServiceBusTrigger("posttopic", "postsubscription", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a User object
                var personthatmadepostdetails = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(message.Body.ToArray()));

                // Fetch all subscriptions for the specified target user
                var allSubscriptions = await _dbContext.Subscriptions
                    .Where(s => s.NotificationTargetUserId == personthatmadepostdetails.UserId)
                    .ToListAsync();

                // send email notification to all subscribers
                foreach (var subscriberUser in allSubscriptions)
                {
                    // Fetch the associated user for the subscription
                    var subscriber = await _dbContext.Users.FindAsync(subscriberUser.SubscriberUserId);
            
                    if (subscriber == null)
                    {
                        _logger.LogError($"Subscriber not found for user ID: {subscriberUser.SubscriberUserId}");
                        continue;
                    }

                    var person = await _dbContext.Users.FindAsync(subscriberUser.SubscriberUserId);
                    if (person != null)
                    {
                        // Send notification to the subscriber
                        await SendNotification(person);
                        // await SendNotification(subscriber, newPostData);

                        _logger.LogInformation($"Notification sent to {person.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notifications: {ex.Message}");
            }

            // Complete the message
        await messageActions.CompleteMessageAsync(message);
        }

        private async Task SendNotification(User subscriber)
        // private async Task SendNotification(User subscriber, ServiceBusReceivedMessage messagee)
        {
            // // Deserialize the Service Bus message body to a Subscription object
            // var subscription = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(messagee.Body.ToArray()));

            try
            {
                // Create email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", ""));
                message.To.Add(new MailboxAddress(subscriber.FirstName, subscriber.Email ));
                message.Subject = "New Post Notification";
                message.Body = new TextPart("plain")
                {
                    Text = $"Hi {subscriber.FirstName},\n\nA new post has been published.\n\nRegards,\nCrispy Doodle Team"
                };

                // Connect to SMTP server and send email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync("", "");
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

        
        
    }
}
