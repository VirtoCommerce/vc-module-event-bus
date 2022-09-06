using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class ConnectionsController : ControllerBase
    {

        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;
        private readonly IEventBusProviderConnectionsService _eventBusProviderConnectionsService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;
        private readonly ICrudService<ProviderConnection> _providerConnectionCrudService;        
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;

        public ConnectionsController(
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            IEventBusProviderConnectionsService eventBusProviderConnectionsService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService,
            ICrudService<ProviderConnection> providerConnectionCrudService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService
            )
        {
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _eventBusProviderConnectionsService = eventBusProviderConnectionsService;
            _subscriptionSearchService = subscriptionSearchService;
            _providerConnectionCrudService = providerConnectionCrudService;
            _providerConnectionSearchService = providerConnectionSearchService;
        }

        /// <summary>
        /// Search for existing connections
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [HttpPost("connections/search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ProviderConnectionSearchResult>> SearchConnections([FromBody] ProviderConnectionSearchCriteria searchCriteria)
        {
            var searchResult = await _providerConnectionSearchService.SearchAsync(searchCriteria);

            var cfgConnections = _eventBusReadConfigurationService.GetProviderConnections().AsEnumerable();
            if (!searchCriteria.Name.IsNullOrEmpty())
            {
                cfgConnections = cfgConnections.Where(x => x.Name == searchCriteria.Name);
            }
            if (!searchCriteria.ProviderName.IsNullOrEmpty())
            {
                cfgConnections = cfgConnections.Where(x => x.ProviderName == searchCriteria.ProviderName);
            }
            var totalResult = new ProviderConnectionSearchResult()
            {
                TotalCount = searchResult.TotalCount + (cfgConnections?.Count() ?? 0),
                Results = new List<ProviderConnection>()
            };

            totalResult.Results.AddRange(cfgConnections);
            totalResult.Results.AddRange(searchResult.Results);

            return Ok(totalResult);
        }

        /// <summary>
        /// Get existing connection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("connections/{name}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ProviderConnection>> GetConnectionByName(string name)
        {
            var conn = await _eventBusProviderConnectionsService.GetProviderConnectionAsync(name);

            return Ok(conn);
        }

        /// <summary>
        /// Create new provider connection in the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("connections")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult> CreateConnection([FromBody] ProviderConnectionRequest request)
        {
            await _providerConnectionCrudService.SaveChangesAsync(new List<ProviderConnection>() { request.ToModel() });
            return Ok();
        }

        /// <summary>
        /// Update existing connection (DB-registered only)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("connections")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult> UpdateConnection([FromBody] ProviderConnectionRequest request)
        {
            var conn = await _eventBusProviderConnectionsService.GetProviderConnectionAsync(request.Name);
            if (conn != null && conn.Id != null)
            {
                var connToUpdate = request.ToModel();
                connToUpdate.Id = conn.Id;
                await _providerConnectionCrudService.SaveChangesAsync(new List<ProviderConnection>() { connToUpdate });
            }
            return Ok();
        }

        /// <summary>
        /// Delete existing connection by name (DB-registered only)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("connections/{name}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteConnection(string name)
        {
            var subscriptions = await _subscriptionSearchService.SearchAsync(new SubscriptionSearchCriteria() { ConnectionName = name });

            if (subscriptions.TotalCount != 0)
            {
                throw new PlatformException($@"Can't delete provider connection {name}. Please remove related subscriptions first.");
            }

            var conn = await _eventBusProviderConnectionsService.GetProviderConnectionAsync(name);
            if (conn != null && conn.Id != null)
            {
                await _providerConnectionCrudService.DeleteAsync(new[] { conn.Id });
            }
            return Ok();
        }
    }
}
