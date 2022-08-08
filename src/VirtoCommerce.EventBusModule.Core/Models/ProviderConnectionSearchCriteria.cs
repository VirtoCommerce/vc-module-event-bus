using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string Provider { get; set; }
    }
}
