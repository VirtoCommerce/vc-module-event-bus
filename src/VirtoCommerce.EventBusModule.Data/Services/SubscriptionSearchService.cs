using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Caching;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class SubscriptionSearchService : ISubscriptionSearchService
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SubscriptionSearchService(ISubscriptionService subscriptionService, Func<ISubscriptionRepository> subscriptionRpositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _subscriptionService = subscriptionService;
            _subscriptionRepositoryFactory = subscriptionRpositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<SubscriptionSearchResult> SearchAsync(SubscriptionSearchCriteria searchCriteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchAsync), searchCriteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SubscriptionSearchCacheRegion.CreateChangeToken());
                var result = new SubscriptionSearchResult();

                using (var repository = _subscriptionRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = BuildSortExpression(searchCriteria);
                    var query = BuildQuery(searchCriteria, repository);

                    result.TotalCount = await query.CountAsync();

                    if (searchCriteria.Take > 0 && result.TotalCount > 0)
                    {
                        var subscriptionIds = query.OrderBySortInfos(sortInfos)
                                            .ThenBy(x => x.Id)
                                            .Select(x => x.Id)
                                            .Skip(searchCriteria.Skip)
                                            .Take(searchCriteria.Take)
                                            .ToArray();

                        result.Results = (await _subscriptionService.GetByIdsAsync(subscriptionIds, searchCriteria.ResponseGroup)).OrderBy(x => Array.IndexOf(subscriptionIds, x.Id)).ToArray();
                    }
                }

                return result;
            });
        }


        protected virtual IQueryable<SubscriptionEntity> BuildQuery(SubscriptionSearchCriteria searchCriteria, ISubscriptionRepository repository)
        {
            var query = repository.Subscriptions;

            if (!searchCriteria.EventIds.IsNullOrEmpty())
            {
                query = query.Where(x => x.Events.Any(y => searchCriteria.EventIds.Contains(y.EventId)));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(SubscriptionSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(SubscriptionEntity.CreatedDate) }
                };
            }

            return sortInfos;
        }
    }
}
