using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using VirtoCommerce.EventBusModule.Core.JsonConverters;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.Platform.Core.Common;
using System.ComponentModel;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnection : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public JObject ConnectionOptions { get; private set; }
        public string ConnectionOptionsSerialized
        {
            get {
                return ConnectionOptions?.ToString();
            }
            set {
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
