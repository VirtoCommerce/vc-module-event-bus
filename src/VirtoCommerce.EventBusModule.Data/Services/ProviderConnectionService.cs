using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Events;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class ProviderConnectionService : CrudService<ProviderConnection, ProviderConnectionEntity, ProviderConnectionChangeEvent, ProviderConnectionChangedEvent>, IProviderConnectionService
    {
        public ProviderConnectionService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        protected override Task<IList<ProviderConnectionEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((IEventBusRepository)repository).GetProviderConnectionsByIdsAsync(ids);
        }
    }
}
