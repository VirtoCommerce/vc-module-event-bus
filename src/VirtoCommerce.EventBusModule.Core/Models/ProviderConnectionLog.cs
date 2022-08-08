using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionLog : AuditableEntity, ICloneable
    {
        public string ProviderName { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as ProviderConnectionLog;
            return result;
        }
    }
}
