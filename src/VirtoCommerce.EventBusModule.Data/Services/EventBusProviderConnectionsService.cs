using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusProviderConnectionsService : IEventBusProviderConnectionsService
    {
        private ConcurrentDictionary<string, EventBusProvider> Connections { get; } = new ConcurrentDictionary<string, EventBusProvider>();

        private readonly IEventBusProviderService _eventBusProviderService;
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;
        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;

        public EventBusProviderConnectionsService(
            IEventBusProviderService eventBusProviderService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService,
            IEventBusReadConfigurationService eventBusReadConfigurationService)
        {
            _eventBusProviderService = eventBusProviderService;
            _providerConnectionSearchService = providerConnectionSearchService;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
        }

        public EventBusProvider GetConnectedProvider(string providerConnectionName)
        {
            if (!Connections.ContainsKey(providerConnectionName))
            {
                var connection = GetProviderConnection(providerConnectionName);

                var eventBusProvider = _eventBusProviderService.CreateProvider(connection.ProviderName);
                eventBusProvider.SetConnectionOptions(connection.ConnectionOptions);
                Connections[providerConnectionName] = eventBusProvider;
            }

            var result = Connections[providerConnectionName];
            if (!result.IsConnected()) result.Connect();

            return result;
        }

        public ProviderConnection GetProviderConnection(string providerConnectionName)
        {
            // Connection from appsettings have priority
            var connection = _eventBusReadConfigurationService.GetProviderConnection(providerConnectionName);
            connection ??= _providerConnectionSearchService.SearchAsync(new ProviderConnectionSearchCriteria() { Name = providerConnectionName }).Result.Results.FirstOrDefault();
            if (connection == null)
            {
                throw new PlatformException($@"The provider connection {providerConnectionName} is not registered");
            }
            return connection;
        }
    }
}
