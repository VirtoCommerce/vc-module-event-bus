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
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class EventBusController : ControllerBase
    {
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsManager _eventBusSubscriptionsManager;
        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;
        private readonly ICrudService<Subscription> _subscriptionService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;
        private readonly ICrudService<ProviderConnection> _providerConnectionService;
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;
        private readonly ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog> _providerConnectionLogSearchService;

        public EventBusController(RegisteredEventService registeredEventService,
            IEventBusSubscriptionsManager eventBusSubscriptionsManager,
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            ICrudService<Subscription> subscriptionService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService,
            ICrudService<ProviderConnection> providerConnectionService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService,
            ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog> providerConnectionLogSearchService)
        {
            _registeredEventService = registeredEventService;
            _eventBusSubscriptionsManager = eventBusSubscriptionsManager;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _providerConnectionService = providerConnectionService;
            _providerConnectionSearchService = providerConnectionSearchService;
            _providerConnectionLogSearchService = providerConnectionLogSearchService;
        }

        /// <summary>
        /// Enlist all domain events page by page
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet("events")]
        [Authorize(ModuleConstants.Security.Permissions.ReadEvent)]
        public ActionResult<string[]> Get(int skip, int take = 20)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            return Ok(allEvents.Skip(skip).Take(take).ToArray());
        }

        /// <summary>
        /// Search for existing subscriptions (DB registered + configuration registered)
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [HttpPost("subscriptions/search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<SubscriptionSearchResult>> SearchSubscriptions([FromBody] SubscriptionSearchCriteria searchCriteria)
        {
            var searchResult = await _subscriptionSearchService.SearchAsync(searchCriteria);
            var cfgSubscriptions = _eventBusReadConfigurationService.GetSubscriptions().AsEnumerable();
            if (!searchCriteria.Name.IsNullOrEmpty())
            {
                cfgSubscriptions = cfgSubscriptions.Where(x => x.Name == searchCriteria.Name);
            }
            if (!searchCriteria.ConnectionName.IsNullOrEmpty())
            {
                cfgSubscriptions = cfgSubscriptions.Where(x => x.ConnectionName == searchCriteria.ConnectionName);
            }
            searchResult.TotalCount += cfgSubscriptions.Count();
            searchResult.Results.AddRange(cfgSubscriptions);

            return Ok(searchResult);
        }

        /// <summary>
        /// Get specific subscription (DB-registered only)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<Subscription>> GetSubscriptionById(string id)
        {
            var subscriptions = await _subscriptionService.GetAsync(new List<string> { id });
            return Ok(subscriptions.FirstOrDefault());
        }

        /// <summary>
        /// Register new subscription in the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("subscriptions")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> CreateSubscription([FromBody] SubscriptionRequest request)
        {
            var result = await _eventBusSubscriptionsManager.SaveSubscriptionAsync(request);
            return Ok(result?.Id);
        }

        /// <summary>
        /// Update existing subscription (DB-registered only)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("subscriptions")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public Task UpdateSubscription([FromBody] SubscriptionRequest request)
        {
            return _eventBusSubscriptionsManager.SaveSubscriptionAsync(request);
        }

        /// <summary>
        /// Delete existing subscription (DB-registered only)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public Task DeleteSubscription(string id)
        {
            return _subscriptionService.DeleteAsync(new[] { id });
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
        /// Search for provider connection logs/fails
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [HttpPost("logs/search")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> SearchProviderConnectionLog([FromBody] ProviderConnectionLogSearchCriteria searchCriteria)
        {
            var result = await _providerConnectionLogSearchService.SearchAsync(searchCriteria);
            return Ok(result);
        }
    }
}
