using System;
using System.Text;
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
            AppDbContext dbContext,
            ILogger<NewSubscriptionFunction> logger)
        {
            logger.LogInformation("Message ID: {id}", message.MessageId);
            logger.LogInformation("Message Body: {body}", message.Body);
            logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a Subscription object
                var subscription = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(message.Body));

                // Add the new subscription to the DbContext
                await dbContext.Subscriptions.AddAsync(subscription);

                // Save changes in the DbContext to Cosmos DB
                await dbContext.SaveChangesAsync();

                // logger.LogInformation($"Subscription for user {subscription.SubscriberUserId} to post {subscription.PostId} stored in Cosmos DB.");
                logger.LogInformation("New subscription added to Cosmos DB");
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
