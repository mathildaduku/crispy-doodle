using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Models
{
    public class Subscription
    {
        public Guid SubsciptionId { get; set; }
        public Guid SubscriberUserId { get; set; }
        public Guid NotificationTargetUserId { get; set; }
    }
}
