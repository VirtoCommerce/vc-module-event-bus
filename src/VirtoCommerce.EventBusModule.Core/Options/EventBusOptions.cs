using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Options
{
    public class EventBusOptions
    {
        public IReadOnlyCollection<Subscription> Subscriptions { get; set; }
        public IReadOnlyCollection<ProviderConnection> Connections { get; set; }
    }
}
