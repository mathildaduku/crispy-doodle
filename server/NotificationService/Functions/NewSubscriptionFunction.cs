using System.Text;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Helpers;
using NotificationService.Models;

namespace NotificationService
{
    public class NewSubscriptionFunction
    {
        private readonly ILogger<NewSubscriptionFunction> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;

        public NewSubscriptionFunction(ILogger<NewSubscriptionFunction> logger, ISubscriptionService subscriptionService, IMapper mapper)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _mapper = mapper;
        }

        [Function(nameof(NewSubscriptionFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/subscriptioncreated", "notification-subscription-created", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", Encoding.UTF8.GetString(message.Body.ToArray()));
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the Service Bus message body to a Subscription object
                var serviceBusMessage = JsonConvert.DeserializeObject<CustomServiceBusMessage<SubscriptionCreated>>(Encoding.UTF8.GetString(message.Body.ToArray()));
                var subscriptionCreatedMessage = serviceBusMessage.Message;
            
            if (subscriptionCreatedMessage != null)
            {
                // Map the SubscriptionCreated object to a Subscription object
                var subscription = _mapper.Map<Subscription>(subscriptionCreatedMessage);

                // Add the new subscription to the DbContext
                await _subscriptionService.AddSubscriptionAsync(subscription);
                
                _logger.LogInformation("New subscription added to Cosmos DB");
            } 
            else
            {
                _logger.LogWarning("Invalid subscription object");
            }
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
