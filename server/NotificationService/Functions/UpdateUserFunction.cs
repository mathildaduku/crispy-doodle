using System.Text;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Helpers;
using NotificationService.Models;

namespace NotificationService.Functions
{
    public class UpdateUserFunction
    {
        private readonly ILogger<UpdateUserFunction> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UpdateUserFunction(ILogger<UpdateUserFunction> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [Function(nameof(UpdateUserFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/accountupdated", "notification-account-updated", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Deserialize the message body to a User object
                var serviceBusMessage = JsonConvert.DeserializeObject<CustomServiceBusMessage<AccountUpdated>>(Encoding.UTF8.GetString(message.Body.ToArray()));
                var accountUpdatedMessage = serviceBusMessage?.Message;

                if (accountUpdatedMessage != null)
                {
                    var userToUpdate = _mapper.Map<User>(accountUpdatedMessage);
                    // Update the user in the DbContext
                    await _userService.UpdateUserAsync(userToUpdate);

                    _logger.LogInformation($"User with ID '{userToUpdate.UserId}' updated in Cosmos DB.");
                }
                else
                {
                    _logger.LogError("Error processing update user: User object is null.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing update user: {ex.Message}");
            }

             // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
