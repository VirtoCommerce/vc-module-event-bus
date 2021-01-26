using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        public async Task<SubscriptionInfo[]> GetByIdsAsync(string[] ids, string responseGroup = null)
        {
            return await Task.FromResult(new SubscriptionInfo[] { });
        }

        public async Task SaveChangesAsync(SubscriptionInfo[] subscriptionInfos)
        {
            await Task.CompletedTask;
        }

        public Task DeleteByIdsAsync(string[] ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
