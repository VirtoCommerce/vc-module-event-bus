using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface ISubscriptionSearchService
    {
        Task<SubscriptionSearchResult> SearchAsync(SubscriptionSearchCriteria searchCriteria);
    }
}
