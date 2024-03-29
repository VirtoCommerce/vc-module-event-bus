using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusProviderConnectionsService : IEventBusProviderConnectionsService
    {
        private ConcurrentDictionary<string, EventBusProvider> Connections { get; } = new ConcurrentDictionary<string, EventBusProvider>();

        private readonly IEventBusProviderService _eventBusProviderService;
        private readonly IProviderConnectionSearchService _providerConnectionSearchService;
        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;

        public EventBusProviderConnectionsService(
            IEventBusProviderService eventBusProviderService,
            IProviderConnectionSearchService providerConnectionSearchService,
            IEventBusReadConfigurationService eventBusReadConfigurationService)
        {
            _eventBusProviderService = eventBusProviderService;
            _providerConnectionSearchService = providerConnectionSearchService;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
        }

        public async Task<EventBusProvider> GetConnectedProviderAsync(string providerConnectionName)
        {
            if (!Connections.ContainsKey(providerConnectionName))
            {
                var connection = await GetProviderConnectionAsync(providerConnectionName);

                var eventBusProvider = _eventBusProviderService.CreateProvider(connection.ProviderName);
                eventBusProvider.SetConnectionOptions(connection.ConnectionOptions);
                Connections[providerConnectionName] = eventBusProvider;
            }

            var result = Connections[providerConnectionName];
            if (!result.IsConnected())
            {
                result.Connect();
            }

            return result;
        }

        public async Task<ProviderConnection> GetProviderConnectionAsync(string providerConnectionName)
        {
            // Connection from appsettings have priority
            var connection = _eventBusReadConfigurationService.GetProviderConnection(providerConnectionName);
            connection ??= (await _providerConnectionSearchService.SearchAsync(new ProviderConnectionSearchCriteria() { Name = providerConnectionName }))?.Results?.FirstOrDefault();
            return connection;
        }
    }
}
