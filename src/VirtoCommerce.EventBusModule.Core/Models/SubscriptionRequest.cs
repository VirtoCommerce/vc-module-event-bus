using System.Linq;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionRequest
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string JsonPathFilter { get; set; }
        public string PayloadTransformationTemplate { get; set; }
        public JObject EventSettings { get; set; }

        public string[] EventIds { get; set; }


        public Subscription ToModel()
        {
            return new Subscription
            {
                Name = Name,
                ConnectionName = ConnectionName,
                JsonPathFilter = JsonPathFilter,
                PayloadTransformationTemplate = PayloadTransformationTemplate,
                EventSettingsSerialized = EventSettings.ToString(),
                //ConnectionString = ConnectionString,
                //AccessKey = AccessKey,
                Events = EventIds.Select(x => new SubscriptionEvent { EventId = x }).ToArray()
            };
        }
    }
}
