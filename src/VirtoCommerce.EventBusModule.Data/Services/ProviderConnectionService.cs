using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProviderConnectionService : CrudService<ProviderConnection, ProviderConnectionEntity, ProviderConnectionChangeEvent, ProviderConnectionChangedEvent>
    {
        IEventBusProviderConnectionsService _eventBusProviderConnectionsService;

        public ProviderConnectionService(
            Func<IEventBusRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IEventBusProviderConnectionsService eventBusProviderConnectionsService
            ) : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _eventBusProviderConnectionsService = eventBusProviderConnectionsService;
        }

        protected override async Task<IEnumerable<ProviderConnectionEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await ((IEventBusRepository)repository).GetProviderConnectionsByIdsAsync(ids);
        }

        protected override Task AfterDeleteAsync(IEnumerable<ProviderConnection> models, IEnumerable<GenericChangedEntry<ProviderConnection>> changedEntries)
        {
            // Force EventBusConnection to remove from active connections cache
            _ = models.Select(async x => await _eventBusProviderConnectionsService.RemoveConnectedProviderAsync(x.ProviderName));
            return base.AfterDeleteAsync(models, changedEntries);
        }
    }
}
