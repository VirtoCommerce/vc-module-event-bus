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
    public class SubscriptionService : CrudService<Subscription, SubscriptionEntity, SubscriptionChangeEvent, SubscriptionChangedEvent>, ISubscriptionService
    {

        public SubscriptionService(Func<IEventBusRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        protected override Task<IList<SubscriptionEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((IEventBusRepository)repository).GetSubscriptionsByIdsAsync(ids);
        }
    }
}
