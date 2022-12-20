using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string[] EventIds { get; set; }
    }
}
