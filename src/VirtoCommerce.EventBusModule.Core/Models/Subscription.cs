using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class Subscription : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string JsonPathFilter { get; set; }
        public string PayloadTransformationTemplate { get; set; }
        public string EventSettingsSerialized { get; set; }
        public ICollection<SubscriptionEvent> Events { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as Subscription;

            result.Events = Events?.Select(x => x.Clone()).OfType<SubscriptionEvent>().ToList();

            return result;
        }
    }
}
