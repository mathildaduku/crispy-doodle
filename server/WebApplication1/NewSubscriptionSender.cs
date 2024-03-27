using Newtonsoft.Json;
using System.Text;

namespace WebApplication1
{
    public class NewSubscriptionSender
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _topicName;

        public NewSubscriptionSender(string serviceBusConnectionString, string topicName)
        {
            _serviceBusConnectionString = serviceBusConnectionString;
            _topicName = topicName;
        }

        public async Task SendNewSubscriptionAsync(Subscription newSubscription)
        {
            try
            {
                var topicClient = new TopicClient(_serviceBusConnectionString, _topicName);

                // Serialize the new subscription object to JSON
                var messageBody = JsonConvert.SerializeObject(newSubscription);

                // Create a message
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                // Send the message to the Service Bus topic
                await topicClient.SendAsync(message);

                await topicClient.CloseAsync();
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred while sending new subscription message: {ex.Message}");
            }
        }
    }
}
