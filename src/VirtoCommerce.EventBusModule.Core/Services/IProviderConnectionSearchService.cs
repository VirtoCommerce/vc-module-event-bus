using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Core.Services;

public interface IProviderConnectionSearchService : ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection>
{
}
