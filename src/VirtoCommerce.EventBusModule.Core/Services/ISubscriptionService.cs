using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface ISubscriptionService
    {
        Task<SubscriptionInfo[]> GetByIdsAsync(string[] ids, string responseGroup = null);
        Task SaveChangesAsync(SubscriptionInfo[] subscriptionInfos);
        Task DeleteByIdsAsync(string[] ids);
    }
}
