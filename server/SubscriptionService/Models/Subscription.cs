using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models
{
    public class Subscription
    {
        [Key]
        public Guid SubscriptionId { get; set; }
        public string SubscriberUserId { get; set; } // user who subscribes
        public string TargetUserId { get; set; } // user whose posts are being subscribed to
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

