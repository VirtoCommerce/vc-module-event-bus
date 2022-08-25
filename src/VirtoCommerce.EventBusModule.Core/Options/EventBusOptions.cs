using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Options
{
    public class EventBusOptions
    {
        public IList<Subscription> Subscriptions { get; set; }
        public IList<ProviderConnection> Connections { get; set; }
    }
}
