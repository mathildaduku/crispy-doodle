using System;
using System.Text;
using System.Threading.Tasks;
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
        private readonly AppDbContext _dbContext;

        public DeleteUserFunction(ILogger<DeleteUserFunction> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                
                // Remove the user from the DbContext
                _dbContext.Users.Remove(user);

                // Save changes in the DbContext to Cosmos DB
            
                await _dbContext.SaveChangesAsync();
                } catch (Exception ex)
                {
                    _logger.LogError($"Error processing delete user: {ex.Message}");
                }
             // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
