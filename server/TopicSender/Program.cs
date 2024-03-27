﻿using System.Text;
using System.Threading.Tasks;
//using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

class Program
{
    private const string ServiceBusConnectionString = "";
    private const string TopicName = "mytopic";

    static async Task Main(string[] args)
    {
        // Create a new subscription
        var newSubscription = new Subscription
        {
            SubsciptionId = Guid.NewGuid(), // Generate a new unique subscription ID
            SubscriberUserId = Guid.NewGuid(), // ID of the user who subscribed
            NotificationTargetUserId = Guid.NewGuid() // ID of the user who will receive notifications
        };


        // Send the new subscription message
        await SendNewSubscriptionAsync(newSubscription);

        Console.WriteLine("New subscription message sent successfully.");
        Console.ReadLine();
    }

    static async Task SendNewSubscriptionAsync(Subscription newSubscription)
    {
        try
        {
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

public class Subscription
{
  
        public Guid SubsciptionId { get; set; }
        public Guid SubscriberUserId { get; set; }
        public Guid NotificationTargetUserId { get; set; }
    
}


/*// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the topic
ServiceBusSender sender;

// number of messages to be sent to the topic
const int numOfMessages = 3;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//TODO: Replace the "<NAMESPACE-CONNECTION-STRING>" and "<TOPIC-NAME>" placeholders.
client = new ServiceBusClient("");
sender = client.CreateSender("mytopic");

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus topic
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();*/