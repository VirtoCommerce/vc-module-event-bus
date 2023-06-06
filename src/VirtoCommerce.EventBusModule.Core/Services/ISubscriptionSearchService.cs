using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Core.Services;

public interface ISubscriptionSearchService : ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription>
{
}
