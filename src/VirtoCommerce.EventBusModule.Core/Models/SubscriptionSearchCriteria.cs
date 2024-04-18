using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    [SwaggerSchemaId("EventBusSubscriptionSearchCriteria")]
    public class SubscriptionSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string ConnectionName { get; set; }
        public string[] EventIds { get; set; }
    }
}
