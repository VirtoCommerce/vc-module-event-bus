using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class ProviderConnectionLogSearchService : SearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog, ProviderConnectionLogEntity>, IProviderConnectionLogSearchService
    {
        public ProviderConnectionLogSearchService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IProviderConnectionLogService crudService, IOptions<CrudOptions> crudOptions)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
        }

        protected override IQueryable<ProviderConnectionLogEntity> BuildQuery(IRepository repository, ProviderConnectionLogSearchCriteria criteria)
        {
            var query = ((IEventBusRepository)repository).ProviderConnectionLogs.Where(x => (criteria.StartCreatedDate == null || x.ModifiedDate >= criteria.StartCreatedDate) && (criteria.EndCreatedDate == null || x.ModifiedDate <= criteria.EndCreatedDate));

            if (!criteria.ObjectIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.ObjectIds.Contains(x.Id));
            }

            if (!criteria.ProviderConnectionName.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.ProviderConnectionName == x.ProviderName);
            }

            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(ProviderConnectionLogSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(ProviderConnectionLogEntity.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }
    }
}
