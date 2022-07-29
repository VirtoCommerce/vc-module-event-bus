using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionEvent : AuditableEntity, ICloneable
    {
        public string SubscriptionId { get; set; }
        public string EventId { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as SubscriptionEvent;
            return result;
        }
    }
}
