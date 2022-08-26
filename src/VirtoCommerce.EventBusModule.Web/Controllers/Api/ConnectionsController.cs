using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ICrudService<ProviderConnection> _providerConnectionService;
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;

        public ConnectionsController(
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            ICrudService<ProviderConnection> providerConnectionService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService
            )
        {
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _providerConnectionService = providerConnectionService;
            _providerConnectionSearchService = providerConnectionSearchService;
        }

        /// <summary>
        /// Search for existing connections (DB registered + configuration registered)
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
            if (!searchCriteria.Provider.IsNullOrEmpty())
            {
                cfgConnections = cfgConnections.Where(x => x.ProviderName == searchCriteria.Provider);
            }
            searchResult.TotalCount += cfgConnections.Count();
            searchResult.Results.AddRange(cfgConnections);

            return Ok(searchResult);
        }

        /// <summary>
        /// Get existing connection (DB-registered only)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("connections/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ProviderConnection>> GetConnectionById(string id)
        {
            var connections = await _providerConnectionService.GetAsync(new List<string> { id });
            return Ok(connections.FirstOrDefault());
        }

        /// <summary>
        /// Create new provider connection in the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("connections")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> CreateConnection([FromBody] ProviderConnectionRequest request)
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
        public Task UpdateConnection([FromBody] ProviderConnectionRequest request)
        {
            return _providerConnectionService.SaveChangesAsync(new List<ProviderConnection>() { request.ToModel() });
        }

        /// <summary>
        /// Delete existing connection (DB-registered only)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("connections/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public Task DeleteConnection(string id)
        {
            return _providerConnectionService.DeleteAsync(new[] { id });
        }
    }
}
