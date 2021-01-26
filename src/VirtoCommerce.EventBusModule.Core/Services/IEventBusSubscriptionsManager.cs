using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusSubscriptionsManager
    {
        Task<SubscriptionInfo> AddSubscriptionAsync(SubscriptionRequest request);

        void RegisterEvents();
    }
}
