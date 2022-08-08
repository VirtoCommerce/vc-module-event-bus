using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.GenericCrud;
using System.Collections.Generic;

namespace VirtoCommerce.EventBusModule.Web.Controllers.Api
{
    [Route("api/eventbus")]
    [ApiController]
    [Authorize]
    public class EventBusController : ControllerBase
    {
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsManager _eventBusSubscriptionsManager;
        private readonly ICrudService<Subscription> _subscriptionService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;
        private readonly ICrudService<ProviderConnection> _providerConnectionService;
        private readonly ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> _providerConnectionSearchService;

        public EventBusController(RegisteredEventService registeredEventService,
            IEventBusSubscriptionsManager eventBusSubscriptionsManager,
            ICrudService<Subscription> subscriptionService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService,
            ICrudService<ProviderConnection> providerConnectionService,
            ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection> providerConnectionSearchService)
        {
            _registeredEventService = registeredEventService;
            _eventBusSubscriptionsManager = eventBusSubscriptionsManager;
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _providerConnectionService = providerConnectionService;
            _providerConnectionSearchService = providerConnectionSearchService;
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
        public async Task<ActionResult<SubscriptionSearchResult>> SearchSubscriptions([FromBody] SubscriptionSearchCriteria searchCriteria)
        {
            var searchResult = await _subscriptionSearchService.SearchAsync(searchCriteria);
            return Ok(searchResult);
        }

        [HttpGet("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<Subscription>> GetSubscriptionById(string id)
        {
            var subscriptions = await _subscriptionService.GetAsync(new List<string> { id });
            return Ok(subscriptions.FirstOrDefault());
        }

        [HttpPost("subscriptions")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> CreateSubscription([FromBody] SubscriptionRequest request)
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
        public Task UpdateSubscription([FromBody] SubscriptionRequest request)
        {
            return _eventBusSubscriptionsManager.SaveSubscriptionAsync(request);
        }

        [HttpDelete("subscriptions/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public Task DeleteSUbscription(string id)
        {
            return _subscriptionService.DeleteAsync(new[] { id });
        }

        [HttpPut("connections")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public Task UpdateConnection([FromBody] ProviderConnectionRequest request)
        {
            return _providerConnectionService.SaveChangesAsync(new List<ProviderConnection>() { request.ToModel() });
        }

        [HttpDelete("connections/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public Task DeleteConnection(string id)
        {
            return _providerConnectionService.DeleteAsync(new[] { id });
        }

        [HttpPost("connections/search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ProviderConnectionSearchResult>> SearchConnections([FromBody] ProviderConnectionSearchCriteria searchCriteria)
        {
            var searchResult = await _providerConnectionSearchService.SearchAsync(searchCriteria);
            return Ok(searchResult);
        }

        [HttpGet("connections/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<ProviderConnection>> GetConnectionById(string id)
        {
            var connections = await _providerConnectionService.GetAsync(new List<string> { id });
            return Ok(connections.FirstOrDefault());
        }

        [HttpPost("connections")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<string>> CreateConnection([FromBody] ProviderConnectionRequest request)
        {
            await _providerConnectionService.SaveChangesAsync(new List<ProviderConnection>() { request.ToModel() });
            return Ok();
        }

      
    }
}
