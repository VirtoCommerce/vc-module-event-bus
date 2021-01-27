using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusProvider
    {
        Task<SendEventResult> SendEventAsync(SubscriptionInfo subscriptionInfo, IList<EventData> events);
    }
}
