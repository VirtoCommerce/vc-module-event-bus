using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Events
{
    public class ProviderConnectionLogChangeEvent : GenericChangedEntryEvent<ProviderConnectionLog>
    {
        public ProviderConnectionLogChangeEvent(IEnumerable<GenericChangedEntry<ProviderConnectionLog>> changedEntries) : base(changedEntries)
        {
        }
    }
}
