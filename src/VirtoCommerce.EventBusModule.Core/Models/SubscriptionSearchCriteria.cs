using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionSearchCriteria : SearchCriteriaBase
    {
        public string Provider { get; set; }
        public string[] EventIds { get; set; }
    }
}
