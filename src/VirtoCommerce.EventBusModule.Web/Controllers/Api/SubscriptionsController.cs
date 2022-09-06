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
        private readonly IEventBusSubscriptionsService _eventBusSubscriptionsService;
        private readonly ICrudService<Subscription> _subscriptionCrudService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;

        public SubscriptionsController(RegisteredEventService registeredEventService,
            IEventBusSubscriptionsManager eventBusSubscriptionsManager,
            IEventBusReadConfigurationService eventBusReadConfigurationService,
            IEventBusSubscriptionsService eventBusSubscriptionsService,
            ICrudService<Subscription> subscriptionCrudService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService
            )
        {
            _registeredEventService = registeredEventService;
            _eventBusSubscriptionsManager = eventBusSubscriptionsManager;
            _eventBusReadConfigurationService = eventBusReadConfigurationService;
            _eventBusSubscriptionsService = eventBusSubscriptionsService;
            _subscriptionCrudService = subscriptionCrudService;
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

            var totalResult = new SubscriptionSearchResult()
            {
                TotalCount = searchResult.TotalCount + (cfgSubscriptions?.Count() ?? 0),
                Results = new List<Subscription>()
            };

            totalResult.Results.AddRange(cfgSubscriptions);
            totalResult.Results.AddRange(searchResult.Results);

            return Ok(totalResult);
        }

        /// <summary>
        /// Get specific subscription by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("subscriptions/{name}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<Subscription>> GetSubscriptionById(string name)
        {
            var subscription = await _eventBusSubscriptionsService.GetSubscriptionAsync(name);
            return Ok(subscription);
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
        public async Task<ActionResult> UpdateSubscription([FromBody] SubscriptionRequest request)
        {
            var subscription = await _eventBusSubscriptionsService.GetSubscriptionAsync(request.Name);
            if (subscription != null && subscription.Id != null)
            {
                var subscrToUpdate = request.ToModel();
                subscrToUpdate.Id = subscription.Id;
                await _subscriptionCrudService.SaveChangesAsync(new List<Subscription>() { subscrToUpdate });
            }
            return Ok();
        }

        /// <summary>
        /// Delete existing subscription by name (DB-registered only)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("subscriptions/{name}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteSubscription(string name)
        {
            var subscription = await _eventBusSubscriptionsService.GetSubscriptionAsync(name);
            if (subscription != null && subscription.Id != null)
            {
                await _subscriptionCrudService.DeleteAsync(new[] { subscription.Id });
            }
            return Ok();
        }
    }
}
