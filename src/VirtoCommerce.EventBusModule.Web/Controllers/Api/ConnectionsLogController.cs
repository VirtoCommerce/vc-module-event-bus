using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class ConnectionsLogController : ControllerBase
    {
        private readonly IProviderConnectionLogSearchService _providerConnectionLogSearchService;

        public ConnectionsLogController(IProviderConnectionLogSearchService providerConnectionLogSearchService)
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
