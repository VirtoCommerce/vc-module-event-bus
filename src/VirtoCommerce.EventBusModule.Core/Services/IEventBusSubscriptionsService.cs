using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    /// <summary>
    /// Get subscriptions from CRUD or application settings
    /// </summary>
    public interface IEventBusSubscriptionsService
    {
        /// <summary>
        /// Get the subscription from CRUD or application settings
        /// </summary>
        public Subscription GetSubscription(string subscriptionName);

        public IList<Subscription> GetSubscriptionByEventId(string eventId);
    }
}
