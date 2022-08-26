using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
    }
}
