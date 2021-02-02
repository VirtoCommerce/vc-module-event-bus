using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class EventBusController : ControllerBase
    {
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsManager _eventBusSubscriptionsManager;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionSearchService _subscriptionSearchService;

        public EventBusController(RegisteredEventService registeredEventService,
            IEventBusSubscriptionsManager eventBusSubscriptionsManager,
            ISubscriptionService subscriptionService,
            ISubscriptionSearchService subscriptionSearchService)
        {
            _registeredEventService = registeredEventService;
            _eventBusSubscriptionsManager = eventBusSubscriptionsManager;
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
        }

        [HttpGet("events")]
        [Authorize(ModuleConstants.Security.Permissions.ReadEvent)]
        public ActionResult<string[]> Get(int skip, int take = 20)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            return Ok(allEvents.Skip(skip).Take(take).ToArray());
        }

        [HttpPost("subscriptions/search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<SubscriptionSearchResult>> Search([FromBody] SubscriptionSearchCriteria searchCriteria)
        {
            var searchResult = await _subscriptionSearchService.SearchAsync(searchCriteria);
            return Ok(searchResult);
        }

        [HttpGet("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<SubscriptionInfo>> GetById(string id)
        {
            var subscriptions = await _subscriptionService.GetByIdsAsync(new[] { id });
            return Ok(subscriptions.FirstOrDefault());
        }

        [HttpPost("subscriptions")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> Create([FromBody] SubscriptionRequest request)
        {
            var limit = 20;
            var searchResult = await _subscriptionSearchService.SearchAsync(new SubscriptionSearchCriteria { Skip = 0, Take = 0 });
            if (searchResult.TotalCount < limit)
            {
                var result = await _eventBusSubscriptionsManager.SaveSubscriptionAsync(request);
                return Ok(result?.Id);
            }
            else
            {
                throw new PlatformException($"The subscription is not created, there are subscriptions almost more then {limit}. Please delete some subscriptions.");
            }
        }

        [HttpPut("subscriptions")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public Task Update([FromBody] SubscriptionRequest request)
        {
            return _eventBusSubscriptionsManager.SaveSubscriptionAsync(request);
        }

        [HttpDelete("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public Task Delete(string id)
        {
            return _subscriptionService.DeleteByIdsAsync(new[] { id });
        }
    }
}
