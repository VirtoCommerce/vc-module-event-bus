using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class Subscription : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string JsonPathFilter { get; set; } = "$"; // All events are good by default
        public string PayloadTransformationTemplate { get; set; } = string.Empty; // No transformation by default

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public JObject EventSettings { get; private set; }
        public string EventSettingsSerialized
        {
            get
            {
                return EventSettings?.ToString(Newtonsoft.Json.Formatting.None);
            }
            set
            {
                EventSettings = value.IsNullOrEmpty() ? null : JObject.Parse(value);
            }
        }
        public ICollection<SubscriptionEvent> Events { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as Subscription;

            result.Events = Events?.Select(x => x.Clone()).OfType<SubscriptionEvent>().ToList();

            return result;
        }
    }
}
