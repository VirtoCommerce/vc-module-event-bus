using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Events
{
    public class ProviderConnectionLogChangedEvent : GenericChangedEntryEvent<ProviderConnectionLog>
    {
        public ProviderConnectionLogChangedEvent(IEnumerable<GenericChangedEntry<ProviderConnectionLog>> changedEntries) : base(changedEntries)
        {
        }
    }
}
