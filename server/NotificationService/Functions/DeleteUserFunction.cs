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
    public class DeleteUserFunction
    {
        private readonly ILogger<DeleteUserFunction> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public DeleteUserFunction(ILogger<DeleteUserFunction> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [Function(nameof(DeleteUserFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/accountdeleted", "notification-account-deleted", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try{
                // Deserialize the message body to a User object
                var serviceBusMessage = JsonConvert.DeserializeObject<CustomServiceBusMessage<AccountDeleted>>(Encoding.UTF8.GetString(message.Body.ToArray()));
                var accountDeletedMessage = serviceBusMessage.Message;
                if (accountDeletedMessage != null)
                {
                    var user = _mapper.Map<User>(accountDeletedMessage);
                    
                    // Remove the user from the DbContext
                    await _userService.DeleteUserAsync(user);

                    _logger.LogInformation($"User with ID '{user.UserId}' deleted from Cosmos DB.");
                }
                else
                {
                    _logger.LogError("Error processing delete user: User object is null.");
                }
 
                } catch (Exception ex)
                {
                    _logger.LogError($"Error processing delete user: {ex.Message}");
                }
             // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
