using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public abstract class EventBusProvider
    {
        public abstract Task<SendEventResult> SendEventAsync(SubscriptionInfo subscriptionInfo, IList<EventData> events);
    }
}
