using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Models;

namespace NotificationService
{
    public class NewUserFunction
    {
        private readonly ILogger<NewUserFunction> _logger;
        private readonly IUserService _userService;

        public NewUserFunction(ILogger<NewUserFunction> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Function(nameof(NewUserFunction))]
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
                // Deserialize the message body to a User object
                var newUser = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(message.Body.ToArray()));

                if (newUser != null)
                {
                    // Add the user to the DbContext
                    await _userService.AddUserAsync(newUser);

                    _logger.LogInformation($"New user with ID '{newUser.UserId}' stored in Cosmos DB.");
                }
                else
                {
                    _logger.LogError("Error processing new user: User object is null.");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error processing new user: {ex.Message}");
            }
             // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
