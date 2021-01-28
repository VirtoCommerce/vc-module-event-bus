using System.Linq;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionRequest
    {
        public string SubscriptionId { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string AccessKey { get; set; }
        public string[] EventIds { get; set; }


        public SubscriptionInfo ToModel()
        {
            return new SubscriptionInfo
            {
                Id = SubscriptionId,
                Provider = Provider,
                ConnectionString = ConnectionString,
                Events = EventIds.Select(x => new SubscriptionEvent { EventId = x }).ToArray()
            };
        }
    }
}
