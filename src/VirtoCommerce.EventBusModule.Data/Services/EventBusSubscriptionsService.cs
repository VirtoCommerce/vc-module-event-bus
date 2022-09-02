using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusSubscriptionsService : IEventBusSubscriptionsService
    {
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;
        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;

        public EventBusSubscriptionsService(
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService,
            IEventBusReadConfigurationService eventBusReadConfigurationService)
        {
            _subscriptionSearchService = subscriptionSearchService;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
        }

        public async Task<Subscription> GetSubscriptionAsync(string subscriptionName)
        {
            var subscription = _eventBusReadConfigurationService.GetSubscription(subscriptionName);
            subscription ??= (await _subscriptionSearchService.SearchAsync(new SubscriptionSearchCriteria() { Name = subscriptionName })).Results.FirstOrDefault();

            return subscription;
        }

        public async Task<IList<Subscription>> GetSubscriptionsByEventIdAsync(string eventId)
        {
            var resultFromAppSettings = _eventBusReadConfigurationService.GetSubscriptionsByEventId(eventId);

            var criteria = new SubscriptionSearchCriteria()
            {
                EventIds = new[] { eventId },
                Skip = 0,
                Take = int.MaxValue,
            };

            var resultFromDatabase = (await _subscriptionSearchService.SearchAsync(criteria)).Results;

            return resultFromAppSettings.Union(resultFromDatabase.Where(x => !resultFromAppSettings.Any(y => y.Name == x.Name))).ToList();
        }

        public async Task<IList<Subscription>> GetSubscriptionsByConnectionNameAsync(string connectionName)
        {
            var resultFromAppSettings = _eventBusReadConfigurationService.GetSubscriptions().Where(x => x.ConnectionName == connectionName);

            var criteria = new SubscriptionSearchCriteria()
            {
                ConnectionName = connectionName,
                Skip = 0,
                Take = int.MaxValue,
            };

            var resultFromDatabase = (await _subscriptionSearchService.SearchAsync(criteria)).Results;

            return resultFromAppSettings.Union(resultFromDatabase).ToList();
        }
    }
}
