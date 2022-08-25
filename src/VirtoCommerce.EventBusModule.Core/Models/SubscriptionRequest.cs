using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionRequest
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string JsonPathFilter { get; set; } = "$";
        public string PayloadTransformationTemplate { get; set; } = string.Empty;
        public string EventSettingsSerialized { get; set; } = "{}";
        public List<SubscriptionEventRequest> Events { get; set; }
        public Subscription ToModel()
        {
            return new Subscription
            {
                Name = Name,
                ConnectionName = ConnectionName,
                JsonPathFilter = JsonPathFilter,
                PayloadTransformationTemplate = PayloadTransformationTemplate,
                EventSettingsSerialized = EventSettingsSerialized,
                Events = Events.Select(x => new SubscriptionEvent { EventId = x.EventId }).ToArray()
            };
        }
    }
}
