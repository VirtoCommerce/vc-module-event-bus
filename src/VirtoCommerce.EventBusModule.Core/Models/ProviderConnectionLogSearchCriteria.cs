using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionLogSearchCriteria : SearchCriteriaBase
    {
        public string ProviderConnectionName { get; set; }
        public DateTime? StartCreatedDate { get; set; }
        public DateTime? EndCreatedDate { get; set; }

    }
}
