using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.EventBusModule.Core.Events;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Caching;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class SubscriptionService : CrudService<Subscription, SubscriptionEntity, SubscriptionChangeEvent, SubscriptionChangedEvent>
    {
        
        public SubscriptionService(Func<IRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher) : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        protected override async Task<IEnumerable<SubscriptionEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await ((IEventBusRepository)repository).GetSubscriptionsByIdsAsync(ids);
        }
    }
}
