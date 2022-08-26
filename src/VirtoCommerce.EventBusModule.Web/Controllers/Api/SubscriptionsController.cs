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
    public class SubscriptionsController : ControllerBase
    {
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsManager _eventBusSubscriptionsManager;
        private readonly IEventBusReadConfigurationService _eventBusReadConfigurationService;
        private readonly ICrudService<Subscription> _subscriptionService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;

        public SubscriptionsController(RegisteredEventService registeredEventService,
            IEventBusSubscriptionsManager eventBusSubscriptionsManager,
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            ICrudService<Subscription> subscriptionService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService
            )
        {
            _registeredEventService = registeredEventService;
            _eventBusSubscriptionsManager = eventBusSubscriptionsManager;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
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
    }
}
