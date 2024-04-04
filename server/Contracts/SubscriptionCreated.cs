using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class SubscriptionCreated
    {
        public Guid SubscriptionId { get; set; }
        public Guid SubscriberUserId { get; set; }
        public Guid TargetUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
