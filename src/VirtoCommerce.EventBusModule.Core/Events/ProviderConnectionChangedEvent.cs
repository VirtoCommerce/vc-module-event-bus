using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Events
{
    public class ProviderConnectionChangedEvent : GenericChangedEntryEvent<ProviderConnection>
    {
        public ProviderConnectionChangedEvent(IEnumerable<GenericChangedEntry<ProviderConnection>> changedEntries) : base(changedEntries)
        {
        }
    }
}
