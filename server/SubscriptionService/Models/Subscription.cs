using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models
{
    public class Subscription
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}

