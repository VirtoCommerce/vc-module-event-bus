using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusReadConfigurationService : IEventBusReadConfigurationService
    {

        public EventBusOptions _eventBusOptions { get; set; }

        public EventBusReadConfigurationService(IOptions<EventBusOptions> eventBusOptions)
        {
            _eventBusOptions = eventBusOptions.Value;
        }

        public ProviderConnection GetProviderConnection(string name)
        {
            return _eventBusOptions.Connections.FirstOrDefault(x => x.Name == name);
        }

        public Subscription GetSubscription(string name)
        {
            return _eventBusOptions.Subscriptions.FirstOrDefault(x => x.Name == name);
        }

        public IList<Subscription> GetSubscriptionsByEventId(string eventId)
        {
            return _eventBusOptions.Subscriptions.Where(x => x.Events.Any(y => y.EventId == eventId)).ToList();
        }
    }
}
