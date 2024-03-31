using System.Threading.Tasks;
// using Azure.Messaging.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;

class Program
{
    private const string ServiceBusConnectionString = "";
    private const string TopicName = "posttopic";

    static async Task Main(string[] args)
    {
        // Create a new subscription
        var newSubscription = new User { UserId = Guid.Parse(""), FirstName = "", LastName = "", Email = ""};
        
        // Send the new subscription message
        await SendNewSubscriptionAsync(newSubscription);

        Console.WriteLine("New subscription message sent successfully.");
        Console.ReadLine();
    }

    static async Task SendNewSubscriptionAsync(User newSubscription)
    {
        try
        {

            // Create a new instance of the TopicClient class
            var topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

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

    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Subscription>? Subscriptions { get; set; }
    }

    public class Subscription
    {
        public Guid SubscriptionId { get; set; }
        public Guid SubscriberUserId { get; set; }
        public Guid NotificationTargetUserId { get; set; }
    }
