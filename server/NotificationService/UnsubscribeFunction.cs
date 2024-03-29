using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService
{
    public class UnsubscribeFunction
    {
        private readonly ILogger<UnsubscribeFunction> _logger;
        private readonly AppDbContext _dbContext;

        public UnsubscribeFunction(ILogger<UnsubscribeFunction> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(UnsubscribeFunction))]
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
                // Deserialize the message
                var unsubscribeData = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(message.Body.ToArray()));
                // Handle UserUnfollow event
                await HandleUserUnfollowEvent(unsubscribeData);

                _logger.LogInformation("Subscription status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing unsubscribe event: {ex.Message}");
            }

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }

        private async Task HandleUserUnfollowEvent(Subscription unsubscribeData)
        {
            // Find and update subscription record for the unfollowed user
            var unfollowedUserSubscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == unsubscribeData.SubscriptionId);
            if (unfollowedUserSubscription != null)
            {
                // Deactivate subscription
                _dbContext.Remove(unfollowedUserSubscription);

            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
