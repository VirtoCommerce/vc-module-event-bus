using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class DefaultEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly ICrudService<Subscription> _subscriptionService;
        private readonly ICrudService<ProviderConnectionLog> _providerConnectionLogService;
        private readonly IHandlerRegistrar _eventHandlerRegistrar;
        private readonly RegisteredEventService _registeredEventService;
        private readonly IEventBusSubscriptionsService _subscriptionsService;
        private readonly IEventBusProviderConnectionsService _providerConnections;

        public DefaultEventBusSubscriptionsManager(IHandlerRegistrar eventHandlerRegistrar,
            RegisteredEventService registeredEventService,
            ICrudService<Subscription> subscriptionService,
            ICrudService<ProviderConnectionLog> providerConnectionLogService,
            IEventBusSubscriptionsService subscriptionsService,
            IEventBusProviderConnectionsService providerConnections)
        {
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _registeredEventService = registeredEventService;
            _subscriptionService = subscriptionService;
            _subscriptionsService = subscriptionsService;
            _providerConnectionLogService = providerConnectionLogService;
            _providerConnections = providerConnections;
        }

        #region Subscription

        public virtual async Task<Subscription> SaveSubscriptionAsync(SubscriptionRequest request)
        {
            Subscription result = null;
            if (CheckEvents(request.Events) &&
                _providerConnections.GetProviderConnection(request.ConnectionName)!=null
                )
            {
                result = request.ToModel();
                await _subscriptionService.SaveChangesAsync(new[] { result });
            }

            return result;
        }

        #endregion Subscription

        #region HandleEvent

        public virtual void RegisterEvents()
        {
            var allEvents = _registeredEventService.GetAllEvents();

            foreach (var @event in allEvents)
            {
                InvokeHandler(@event.Type, _eventHandlerRegistrar);
            }
        }


        protected virtual async Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var eventId = domainEvent.GetType().FullName;

            var searchResult = _subscriptionsService.GetSubscriptionByEventId(eventId);

            if (searchResult.Count > 0)
            {
                var logs = new List<ProviderConnectionLog>();
                var domainEventJObject = JObject.FromObject(domainEvent);

                foreach (var subscription in searchResult)
                {
                    var provider = _providerConnections.GetConnectedProvider(subscription.ConnectionName);

                    if (provider != null)
                    {

                        var tokens = domainEventJObject.SelectTokens(subscription.JsonPathFilter);
                        if (tokens.Any())
                        {

                            Event eventData = null;

                            object payload = null;

                            if (!subscription.PayloadTransformationTemplate.IsNullOrEmpty())
                            {
                                var template = Template.Parse(subscription.PayloadTransformationTemplate);
                                payload = JsonConvert.DeserializeObject(template.Render(domainEvent));
                            }
                            else
                            {
                                payload = domainEvent;
                            }

                            eventData = new Event() { Subscription = subscription, Payload = new EventPayload() { EventId = eventId, Arg = payload } };

                            var result = await provider.SendEventsAsync(new List<Event>() { eventData });

                            if (!string.IsNullOrEmpty(result.ErrorMessage))
                            {
                                logs.Add(new ProviderConnectionLog() { ErrorMessage = result.ErrorMessage, Status = result.Status });
                            }
                        }
                    }
                }

                await _providerConnectionLogService.SaveChangesAsync(logs);
            }
        }

        private void InvokeHandler(Type eventType, IHandlerRegistrar registrar)
        {
            var registerExecutorMethod = registrar
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.Name == nameof(IHandlerRegistrar.RegisterHandler))
                .Where(mi => mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Length == 1)
                .Single(mi => mi.GetParameters().Length == 1)
                .MakeGenericMethod(eventType);

            Func<DomainEvent, CancellationToken, Task> del = (x, token) =>
            {
                return HandleEvent(x, token);
            };

            registerExecutorMethod.Invoke(registrar, new object[] { del });
        }

        #endregion HandleEvent


        private bool CheckEvents(IList<SubscriptionEventRequest> eventIds)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (eventIds.All(x => allEvents.Any(e => e.Id == x.EventId)))
            {
                return true;
            }
            else
            {
                var notRegisteredEvents = eventIds.Where(e => !allEvents.Any(all => all.Id == e.EventId)).Select(x=>x.EventId);
                throw new PlatformException($"The events are not registered: {string.Join(",", notRegisteredEvents)}");
            }
        }
    }
}
