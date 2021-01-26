using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Services;


namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
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

        [HttpGet]
        public ActionResult<string[]> Get()
        {
            var allEvents = _registeredEventService.GetAllEvents();
            return Ok(allEvents.Keys);
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionSearchResult>> Search([FromBody] SubscriptionSearchCriteria searchCriteria)
        {
            var searchResult = await _subscriptionSearchService.SearchAsync(searchCriteria);
            return Ok(searchResult);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] SubscriptionRequest request)
        {
            var result = await _eventBusSubscriptionsManager.AddSubscriptionAsync(request);
            return Ok(result?.Id);
        }

        [HttpPut]
        public Task Update([FromBody] SubscriptionInfo subscription)
        {
            return _subscriptionService.SaveChangesAsync(new[] { subscription });
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _subscriptionService.DeleteByIdsAsync(new[] { id });
        }
    }
}
