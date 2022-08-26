using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
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
        private readonly ICrudService<ProviderConnection> _providerConnectionService;
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;

        public ConnectionsController(
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            IEventBusProviderConnectionsService eventBusProviderConnectionsService,
            ICrudService<ProviderConnection> providerConnectionService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService
            )
        {
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _eventBusProviderConnectionsService = eventBusProviderConnectionsService;
            _providerConnectionService = providerConnectionService;
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
            searchResult.TotalCount += cfgConnections.Count();
            searchResult.Results.AddRange(cfgConnections);

            return Ok(searchResult);
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
            await _providerConnectionService.SaveChangesAsync(new List<ProviderConnection>() { request.ToModel() });
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
                await _providerConnectionService.SaveChangesAsync(new List<ProviderConnection>() { connToUpdate });
            }
            return Ok();
        }

        /// <summary>
        /// Delete existing connection (DB-registered only)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("connections/{name}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteConnection(string name)
        {
            var conn = await _eventBusProviderConnectionsService.GetProviderConnectionAsync(name);
            if (conn != null && conn.Id != null)
            {
                await _providerConnectionService.DeleteAsync(new[] { conn.Id });
            }
            return Ok();
        }
    }
}
