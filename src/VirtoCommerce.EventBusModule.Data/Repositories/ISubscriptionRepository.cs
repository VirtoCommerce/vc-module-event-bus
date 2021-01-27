using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public interface ISubscriptionRepository : IRepository
    {
        IQueryable<SubscriptionEntity> Subscriptions { get; }
        IQueryable<SubscriptionEventEntity> SubscriptionEvens { get; }

        Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(string[] ids);
        Task DeleteSubscriptionsByIdsAsync(string[] ids);
    }
}
