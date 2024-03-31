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
    public class NewUserFunction
    {
        private readonly ILogger<NewUserFunction> _logger;
        private readonly AppDbContext _dbContext;

        public NewUserFunction(ILogger<NewUserFunction> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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

                // Add the user to the  DbContext
                _dbContext.Users.Add(newUser);

                // Save changes in the DbContext to Cosmos DB
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"New user with ID '{newUser.UserId}' stored in Cosmos DB.");
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
