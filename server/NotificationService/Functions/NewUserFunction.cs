using System.Text;
using AutoMapper;
using Azure;
using Azure.Messaging.ServiceBus;
using Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Helpers;
using NotificationService.Models;

namespace NotificationService
{
    public class NewUserFunction
    {
        private readonly ILogger<NewUserFunction> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public NewUserFunction(ILogger<NewUserFunction> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [Function(nameof(NewUserFunction))]
        public async Task Run(
            [ServiceBusTrigger("contracts/accountcreated", "notification-account-created", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            
            try
            {
                // Deserialize the message body to a User object
                var serviceBusMessage = JsonConvert.DeserializeObject<CustomServiceBusMessage<AccountCreated>>(Encoding.UTF8.GetString(message.Body.ToArray()));
                var accountCreatedMessage = serviceBusMessage.Message;


                Console.WriteLine(Encoding.UTF8.GetString(message.Body.ToArray()) + "yooo");
                Console.WriteLine(accountCreatedMessage + "yeah");

                if (accountCreatedMessage != null)
                {
                    var newUser = _mapper.Map<User>(accountCreatedMessage);

                    Console.WriteLine(newUser.UserId + "yaga1");
                    Console.WriteLine(newUser.FirstName + "yaga2");
                    Console.WriteLine(newUser.LastName + "yaga3");

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
