using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Events;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class ProviderConnectionLogService : CrudService<ProviderConnectionLog, ProviderConnectionLogEntity, ProviderConnectionLogChangeEvent, ProviderConnectionLogChangedEvent>
    {
        public ProviderConnectionLogService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher) : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        protected override async Task<IEnumerable<ProviderConnectionLogEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await((IEventBusRepository)repository).GetProviderConnectionLogsByIdsAsync(ids);
        }
    }
}
