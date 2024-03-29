using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Events
{
    public class SubscriptionChangedEvent : GenericChangedEntryEvent<Subscription>
    {
        public SubscriptionChangedEvent(IEnumerable<GenericChangedEntry<Subscription>> changedEntries) : base(changedEntries)
        {
        }
    }
}
