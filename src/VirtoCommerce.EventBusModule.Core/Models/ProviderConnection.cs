using System;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnection : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public JObject ConnectionOptions { get; private set; }
        public string ConnectionOptionsSerialized
        {
            get
            {
                return ConnectionOptions?.ToString();
            }
            set
            {
                ConnectionOptions = JObject.Parse(value);
            }
        }
        public object Clone()
        {
            var result = MemberwiseClone() as ProviderConnection;
            return result;
        }
    }
}
