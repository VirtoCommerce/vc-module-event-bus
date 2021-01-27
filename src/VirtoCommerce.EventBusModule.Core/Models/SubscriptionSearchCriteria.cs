using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionSearchCriteria : SearchCriteriaBase
    {
        public string[] EventIds { get; set; }
    }
}
