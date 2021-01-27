using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusSubscriptionsManager
    {
        Task<SubscriptionInfo> SaveSubscriptionAsync(SubscriptionRequest request);

        void RegisterEvents();
    }
}
