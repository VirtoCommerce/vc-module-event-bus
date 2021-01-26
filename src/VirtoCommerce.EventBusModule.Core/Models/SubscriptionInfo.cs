using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionInfo : AuditableEntity
    {
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public SubscriptionEvent[] SubscriptionEvents { get; set; } 
    }
}
