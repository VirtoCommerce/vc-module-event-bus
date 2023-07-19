using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public interface IEventBusRepository : IRepository
    {
        IQueryable<SubscriptionEntity> Subscriptions { get; }
        IQueryable<SubscriptionEventEntity> SubscriptionEvents { get; }
        IQueryable<ProviderConnectionEntity> ProviderConnections { get; }
        IQueryable<ProviderConnectionLogEntity> ProviderConnectionLogs { get; }

        Task<IList<SubscriptionEntity>> GetSubscriptionsByIdsAsync(IList<string> ids);
        Task<IList<ProviderConnectionEntity>> GetProviderConnectionsByIdsAsync(IList<string> ids);
        Task<IList<ProviderConnectionLogEntity>> GetProviderConnectionLogsByIdsAsync(IList<string> ids);
        Task DeleteSubscriptionsByIdsAsync(IList<string> ids);
    }
}
