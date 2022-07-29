using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnection : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionOptionsSerialized { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as ProviderConnection;
            return result;
        }
    }
}
