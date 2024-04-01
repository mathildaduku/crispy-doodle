using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Helpers
{
    internal class CustomServiceBusMessage<T>: ServiceBusMessage 
    {
        public T Message { get; set; }
    }
}
