using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;

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
        public Task<EventBusProvider> GetConnectedProviderAsync(string providerConnectionName);

        public Task<ProviderConnection> GetProviderConnectionAsync(string providerConnectionName);
    }
}
