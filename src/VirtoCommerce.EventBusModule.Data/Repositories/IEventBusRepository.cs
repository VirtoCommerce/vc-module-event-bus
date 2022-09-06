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
        Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(IEnumerable<string> ids);
        Task<ProviderConnectionEntity[]> GetProviderConnectionsByIdsAsync(IEnumerable<string> ids);
        Task<ProviderConnectionLogEntity[]> GetProviderConnectionLogsByIdsAsync(IEnumerable<string> ids);
        Task DeleteSubscriptionsByIdsAsync(IEnumerable<string> ids);
    }
}
