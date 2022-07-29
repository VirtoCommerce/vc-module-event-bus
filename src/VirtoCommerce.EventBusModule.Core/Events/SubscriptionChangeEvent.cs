using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Events
{
    public class SubscriptionChangeEvent : GenericChangedEntryEvent<Subscription>
    {
        public SubscriptionChangeEvent(IEnumerable<GenericChangedEntry<Subscription>> changedEntries) : base(changedEntries)
        {
        }
    }
}
