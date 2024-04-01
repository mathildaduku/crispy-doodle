using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Models;

namespace NotificationService
{
    public class DeleteUserFunction
    {
        private readonly ILogger<DeleteUserFunction> _logger;
        private readonly IUserService _userService;

        public DeleteUserFunction(ILogger<DeleteUserFunction> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Function(nameof(DeleteUserFunction))]
        public async Task Run(
            [ServiceBusTrigger("mytopic", "mysubscription", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try{
                // Deserialize the message body to a User object
                var user = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(message.Body.ToArray()));
                
                if (user != null)
                {
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
