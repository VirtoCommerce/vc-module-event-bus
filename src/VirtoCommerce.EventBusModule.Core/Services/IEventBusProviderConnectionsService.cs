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

        /// <summary>
        /// Forces specified provider to disconnect and remove from the collections cache
        /// </summary>
        /// <param name="providerConnectionName"></param>
        /// <returns></returns>
        public Task RemoveConnectedProviderAsync(string providerConnectionName);
    }
}
