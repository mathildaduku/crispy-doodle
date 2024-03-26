using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService
{
    public class NewSubscriptionFunction
    {
        private readonly ILogger<NewSubscriptionFunction> _logger;
        private readonly AppDbContext _dbContext;

        public NewSubscriptionFunction(ILogger<NewSubscriptionFunction> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(NewSubscriptionFunction))]
        public async Task Run(
            [ServiceBusTrigger("mytopic", "mysubscription", Connection = "myconnectionstring")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions,
            AppDbContext dbContext)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message to a Subscription object
                var subscription = JsonConvert.DeserializeObject<Subscription>(message);

                // Add the new subscription to the DbContext
                await _dbContext.Subscriptions.AddAsync(subscription);

                // Save changes in the DbContext to Cosmos DB
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Subscription for user {subscription.UserId} to post {subscription.PostId} stored in Cosmos DB.");
            }
             
            catch (Exception ex)
            {
                _logger.LogError($"Error processing new subscription: {ex.Message}");
            }
    // Complete the message
    await messageActions.CompleteMessageAsync(message);
        }
    }
}
