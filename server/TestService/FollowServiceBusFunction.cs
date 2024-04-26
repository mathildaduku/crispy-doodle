using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TestService
{
    public class FollowServiceBusFunction
    {
        private readonly ILogger<FollowServiceBusFunction> _logger;

        public FollowServiceBusFunction(ILogger<FollowServiceBusFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FollowServiceBusFunction))]
        public async Task Run(
            [ServiceBusTrigger("FollowTopic", "CrispyDoodle", Connection = "FUNCTIONS_WORKER_RUNTIME")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

             // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
