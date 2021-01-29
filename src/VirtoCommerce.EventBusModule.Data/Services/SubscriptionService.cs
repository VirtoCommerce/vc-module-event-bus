using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Caching;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SubscriptionService(IPlatformMemoryCache platformMemoryCache, Func<ISubscriptionRepository> subscriptionRepositoryFactory)
        {
            _platformMemoryCache = platformMemoryCache;
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
        }

        public async Task<SubscriptionInfo[]> GetByIdsAsync(string[] ids, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", ids));

            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SubscriptionCacheRegion.CreateChangeToken());

                var result = new List<SubscriptionInfo>();

                if (!ids.IsNullOrEmpty())
                {
                    using (var repository = _subscriptionRepositoryFactory())
                    {
                        var entities = await repository.GetSubscriptionsByIdsAsync(ids);

                        if (!entities.IsNullOrEmpty())
                        {
                            result.AddRange(entities.Select(x => x.ToModel(AbstractTypeFactory<SubscriptionInfo>.TryCreateInstance())));
                        }
                    }
                }

                return result.ToArray();
            });
        }

        public async Task SaveChangesAsync(SubscriptionInfo[] subscriptionInfos)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<SubscriptionInfo>>();

            using (var repository = _subscriptionRepositoryFactory())
            {
                var existingIds = subscriptionInfos.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalEntities = await repository.GetSubscriptionsByIdsAsync(existingIds);

                foreach (var subscription in subscriptionInfos)
                {
                    var originalEntity = originalEntities.FirstOrDefault(x => x.Id == subscription.Id);
                    var modifiedEntity = AbstractTypeFactory<SubscriptionEntity>.TryCreateInstance().FromModel(subscription, pkMap);

                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<SubscriptionInfo>(subscription, originalEntity.ToModel(new SubscriptionInfo()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<SubscriptionInfo>(subscription, EntryState.Added));
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                ClearCache();
            }
        }

        public async Task DeleteByIdsAsync(string[] ids)
        {
            using (var repository = _subscriptionRepositoryFactory())
            {
                await repository.DeleteSubscriptionsByIdsAsync(ids);
                await repository.UnitOfWork.CommitAsync();

                ClearCache();
            }
        }

        protected virtual void ClearCache()
        {
            SubscriptionCacheRegion.ExpireRegion();
            SubscriptionSearchCacheRegion.ExpireRegion();
        }
    }
}
