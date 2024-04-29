using Azure.Messaging.ServiceBus;

namespace NotificationService.Helpers
{
    internal class CustomServiceBusMessage<T>: ServiceBusMessage 
    {
        public T Message { get; set; }
    }
}
