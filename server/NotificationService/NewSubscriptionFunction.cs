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
            [ServiceBusTrigger("mytopic", "mysubscription", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", Encoding.UTF8.GetString(message.Body.ToArray()));
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a Subscription object
                var subscription = JsonConvert.DeserializeObject<Subscription>(Encoding.UTF8.GetString(message.Body.ToArray()));

                // Add the new subscription to the DbContext
                _dbContext.Subscriptions.Add(subscription);

                // Save changes in the DbContext to Cosmos DB
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("New subscription added to Cosmos DB");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing new subscription");
            }
            // Complete the message to remove it from the queue
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
