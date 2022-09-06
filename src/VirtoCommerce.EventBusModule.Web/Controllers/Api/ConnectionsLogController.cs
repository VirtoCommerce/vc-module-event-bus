using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class ConnectionsLogController : ControllerBase
    {
        private readonly ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog> _providerConnectionLogSearchService;

        public ConnectionsLogController(ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog> providerConnectionLogSearchService)
        {
            _providerConnectionLogSearchService = providerConnectionLogSearchService;
        }

        /// <summary>
        /// Search for provider connection logs/fails
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [HttpPost("logs/search")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<ProviderConnectionLogSearchResult>> SearchProviderConnectionLog([FromBody] ProviderConnectionLogSearchCriteria searchCriteria)
        {
            var result = await _providerConnectionLogSearchService.SearchAsync(searchCriteria);
            return Ok(result);
        }

    }
}
