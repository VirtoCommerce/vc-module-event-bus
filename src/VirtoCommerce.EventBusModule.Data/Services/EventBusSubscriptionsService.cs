using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Subscription GetSubscription(string subscriptionName)
        {
            var subscription = _eventBusReadConfigurationService.GetSubscription(subscriptionName);
            subscription ??= _subscriptionSearchService.SearchAsync(new SubscriptionSearchCriteria() { Name = subscriptionName }).Result.Results.FirstOrDefault();

            return subscription;
        }

        public IList<Subscription> GetSubscriptionByEventId(string eventId)
        {
            var resultFromAppSettings = _eventBusReadConfigurationService.GetSubscriptionsByEventId(eventId);

            var criteria = new SubscriptionSearchCriteria()
            {
                EventIds = new[] { eventId },
                Skip = 0,
                Take = int.MaxValue,
            };

            var resultFromDatabase = _subscriptionSearchService.SearchAsync(criteria).GetAwaiter().GetResult().Results;
            
            return resultFromAppSettings.Union(resultFromDatabase.Where(x => !resultFromAppSettings.Any(y => y.Name == x.Name))).ToList();
        }

    }
}
