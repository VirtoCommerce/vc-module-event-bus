using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class DefaultEventBusSubscriptionsManager : IEventBusSubscriptionsManager, ICancellableEventHandler<DomainEvent>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IProviderConnectionLogService _providerConnectionLogService;
        private readonly IEventHandlerRegistrar _eventHandlerRegistrar;
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsService _subscriptionsService;
        private readonly IEventBusProviderConnectionsService _providerConnections;
        private readonly ILogger<DefaultEventBusSubscriptionsManager> _logger;

        public DefaultEventBusSubscriptionsManager(
            IEventHandlerRegistrar eventHandlerRegistrar,
            RegisteredEventService registeredEventService,
            ISubscriptionService subscriptionService,
            IProviderConnectionLogService providerConnectionLogService,
            IEventBusSubscriptionsService subscriptionsService,
            IEventBusProviderConnectionsService providerConnections,
            ILogger<DefaultEventBusSubscriptionsManager> logger)
        {
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _registeredEventService = registeredEventService;
            _subscriptionService = subscriptionService;
            _subscriptionsService = subscriptionsService;
            _providerConnectionLogService = providerConnectionLogService;
            _providerConnections = providerConnections;
            _logger = logger;
        }

        #region Subscription

        public virtual async Task<Subscription> SaveSubscriptionAsync(SubscriptionRequest request)
        {
            Subscription result = null;
            if (CheckEvents(request.Events) &&
                await CheckConnection(request.ConnectionName)
                )
            {
                result = request.ToModel();
                await _subscriptionService.SaveChangesAsync([result]);
            }

            return result;
        }

        private async Task<bool> CheckConnection(string connectionName)
        {
            var connection = await _providerConnections.GetProviderConnectionAsync(connectionName);

            return connection == null
                ? throw new PlatformException($"The provider connection {connectionName} is not registered")
                : true;
        }

        #endregion Subscription

        #region HandleEvent

        public virtual void RegisterEvents()
        {
            _eventHandlerRegistrar.RegisterEventHandler(this);
        }

        public Task Handle(DomainEvent message, CancellationToken token = new())
        {
            return HandleEvent(message, token);
        }


        protected virtual async Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var eventId = domainEvent.GetType().FullName;

            var searchResult = await _subscriptionsService.GetSubscriptionsByEventIdAsync(eventId);

            if (searchResult.Count > 0)
            {
                var logs = new List<ProviderConnectionLog>();
                var domainEventJObject = JObject.FromObject(domainEvent);

                foreach (var subscription in searchResult)
                {
                    try
                    {
                        var provider = await _providerConnections.GetConnectedProviderAsync(subscription.ConnectionName);
                        await SendEvent(domainEvent, eventId, logs, domainEventJObject, subscription, provider);
                    }
                    catch (Exception ex)
                    {
                        logs.Add(new ProviderConnectionLog { ProviderName = subscription.ConnectionName, ErrorMessage = ex.ToString() });
                    }
                }

                if (logs.Count > 0)
                {
                    foreach (var log in logs)
                    {
                        _logger.LogError("Failed to send an event. Provider: {Provider}, Status: {Status}, Message: {Message}, Payload: {Payload}",
                            log.ProviderName, log.Status, log.ErrorMessage, log.ErrorPayload);
                    }

                    await _providerConnectionLogService.SaveChangesAsync(logs);
                }
            }
        }

        protected virtual async Task SendEvent(DomainEvent domainEvent, string eventId, List<ProviderConnectionLog> logs, JObject domainEventJObject, Subscription subscription, EventBusProvider provider)
        {
            if (provider != null)
            {

                var tokens = domainEventJObject.SelectTokens(subscription.JsonPathFilter);
                if (tokens.Any())
                {
                    var payload = subscription.PayloadTransformationTemplate.IsNullOrEmpty()
                        ? domainEvent
                        : JsonConvert.DeserializeObject(await Template.Parse(subscription.PayloadTransformationTemplate).RenderAsync(domainEvent));

                    var eventData = new Event
                    {
                        Subscription = subscription,
                        Payload = new EventPayload
                        {
                            EventId = eventId,
                            Arg = payload,
                        }
                    };

                    var result = await provider.SendEventsAsync([eventData]);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        logs.Add(new ProviderConnectionLog
                        {
                            ProviderName = subscription.ConnectionName,
                            Status = result.Status,
                            ErrorMessage = result.ErrorMessage,
                            ErrorPayload = result.ErrorPayload,
                        });
                    }
                }
            }
        }

        #endregion HandleEvent


        protected virtual bool CheckEvents(IList<SubscriptionEventRequest> eventIds)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (eventIds.All(x => allEvents.Any(e => e.Id == x.EventId)))
            {
                return true;
            }

            var notRegisteredEvents = eventIds.Where(e => allEvents.All(x => x.Id != e.EventId)).Select(x => x.EventId);
            throw new PlatformException($"The events are not registered: {string.Join(",", notRegisteredEvents)}");
        }
    }
}
