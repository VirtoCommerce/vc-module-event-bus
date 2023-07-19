using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Core.Services;

public interface IProviderConnectionLogSearchService : ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog>
{
}
