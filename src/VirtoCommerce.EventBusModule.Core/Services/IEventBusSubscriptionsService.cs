using System.Collections.Generic;
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
        public Task<Subscription> GetSubscriptionAsync(string subscriptionName);

        public Task<IList<Subscription>> GetSubscriptionsByEventIdAsync(string eventId);

        public Task<IList<Subscription>> GetSubscriptionsByConnectionNameAsync(string connectionName);
    }
}
