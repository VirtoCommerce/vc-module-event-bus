using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionLog : AuditableEntity, ICloneable
    {
        public string ProviderName { get; set; }
        public int Status { get; set; } = 500;
        public string ErrorMessage { get; set; }
        public string ErrorPayload { get; set; }

        public object Clone()
        {
            var result = (ProviderConnectionLog)MemberwiseClone();
            return result;
        }
    }
}
