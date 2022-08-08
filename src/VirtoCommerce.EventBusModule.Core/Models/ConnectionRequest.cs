using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionRequest
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionOptionsSerialized { get; set; }

        public ProviderConnection ToModel()
        {
            return new ProviderConnection
            {
                Name = Name,
                ProviderName = ProviderName,
                ConnectionOptionsSerialized = ConnectionOptionsSerialized
            };
        }

    }
}
