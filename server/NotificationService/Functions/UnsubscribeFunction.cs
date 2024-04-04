using System.Text;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Helpers;
using NotificationService.Models;

namespace NotificationService
{
    public class UnsubscribeFunction
    {
        private readonly ILogger<UnsubscribeFunction> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;

        public UnsubscribeFunction(ILogger<UnsubscribeFunction> logger, ISubscriptionService subscriptionService, IMapper mapper)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _mapper = mapper;
        }

        [Function(nameof(UnsubscribeFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/subscriptiondeleted", "notification-subscription-deleted", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the message
                var serviceBusMessage = JsonConvert.DeserializeObject<CustomServiceBusMessage<SubscriptionDeleted>>(Encoding.UTF8.GetString(message.Body.ToArray()));
                var subscriptionDeletedMessage = serviceBusMessage.Message;
            if (subscriptionDeletedMessage != null)
            {
                // Map the SubscriptionDeleted object to a Subscription object
                var subscription = _mapper.Map<Subscription>(subscriptionDeletedMessage);


                    // Remove subscription from the DbContext
                    await _subscriptionService.DeleteSubscriptionAsync(subscription);                

                    _logger.LogInformation("Subscription deleted from Cosmos DB");
            }
            else
            {
                _logger.LogWarning("Invalid subscription object");
           
            }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing unsubscribe event: {ex.Message}");
            }

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
