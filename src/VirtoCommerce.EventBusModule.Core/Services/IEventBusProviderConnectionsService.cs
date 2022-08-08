using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    /// <summary>
    /// Get provider connections from CRUD or application settings
    /// </summary>
    public interface IEventBusProviderConnectionsService
    {
        /// <summary>
        /// Get provider connection from CRUD or application settings
        /// </summary>
        /// <param name="providerConnectionName"></param>
        /// <returns></returns>
        public EventBusProvider GetConnectedProvider(string providerConnectionName);
    }
}
