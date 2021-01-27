using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionEvent : AuditableEntity
    {
        public string SubscriptionId { get; set; }
        public string EventId { get; set; }
    }
}
