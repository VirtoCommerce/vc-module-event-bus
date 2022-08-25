using System.Collections.Generic;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    /// <summary>
    /// Read eventbus configurations from applications settings
    /// </summary>
    public interface IEventBusReadConfigurationService
    {
        public Subscription GetSubscription(string name);
        public IList<Subscription> GetSubscriptions();
        public IList<Subscription> GetSubscriptionsByEventId(string eventId);
        public ProviderConnection GetProviderConnection(string name);
        public IList<ProviderConnection> GetProviderConnections();
    }
}
