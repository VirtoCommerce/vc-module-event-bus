using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class ProviderConnectionSearchService : SearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection, ProviderConnectionEntity>
    {
        public ProviderConnectionSearchService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, ICrudService<ProviderConnection> crudService) : base(repositoryFactory, platformMemoryCache, crudService)
        {
        }

        protected override IQueryable<ProviderConnectionEntity> BuildQuery(IRepository repository, ProviderConnectionSearchCriteria criteria)
        {
            var query = ((IEventBusRepository)repository).ProviderConnections;

            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(x => x.Name.Equals(criteria.Name));
            }

            if (!string.IsNullOrEmpty(criteria.ProviderName))
            {
                query = query.Where(x => x.ProviderName.Equals(criteria.ProviderName));
            }
            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(ProviderConnectionSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(ProviderConnectionEntity.Name)
                    }
                };
            }
            return sortInfos;
        }
    }
}
