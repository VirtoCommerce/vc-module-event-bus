using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class SubscriptionSearchService : SearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription, SubscriptionEntity>
    {
        public SubscriptionSearchService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, ICrudService<Subscription> crudService) : base(repositoryFactory, platformMemoryCache, crudService)
        {
        }

        protected override IQueryable<SubscriptionEntity> BuildQuery(IRepository repository, SubscriptionSearchCriteria criteria)
        {
            var query = ((IEventBusRepository)repository).Subscriptions;

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(x => x.Name.Contains(criteria.Name));
            }

            if (!string.IsNullOrEmpty(criteria.ConnectionName))
            {
                query = query.Where(x => x.ConnectionName.Contains(criteria.ConnectionName));
            }

            if (!criteria.EventIds.IsNullOrEmpty())
            {
                query = query.Where(x => x.Events.Any(y => criteria.EventIds.Contains(y.EventId)));
            }

            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(SubscriptionSearchCriteria criteria)
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
