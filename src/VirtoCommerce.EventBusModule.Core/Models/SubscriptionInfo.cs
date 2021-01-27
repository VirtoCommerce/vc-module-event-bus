using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionInfo : AuditableEntity
    {
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string AccessKey { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public SubscriptionEvent[] Events { get; set; }
    }
}
