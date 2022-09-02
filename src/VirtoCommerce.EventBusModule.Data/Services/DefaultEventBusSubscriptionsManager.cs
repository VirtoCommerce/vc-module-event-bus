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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                await CheckConnection(request.ConnectionName)
                )
            {
                result = request.ToModel();
                await _subscriptionService.SaveChangesAsync(new[] { result });
            }

            return result;
        }

        private async Task<bool> CheckConnection(string connectionName)
        {
            var connection = await _providerConnections.GetProviderConnectionAsync(connectionName);
            if (connection == null)
            {
                throw new PlatformException($@"The provider connection {connectionName} is not registered");
            }

            return connection != null;
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
                    catch (Exception exc)
                    {
                        logs.Add(new ProviderConnectionLog() { ProviderName = subscription.ConnectionName, ErrorMessage = exc.ToString() });
                    }                    
                }

                await _providerConnectionLogService.SaveChangesAsync(logs);
            }
        }

        protected virtual async Task SendEvent(DomainEvent domainEvent, string eventId, List<ProviderConnectionLog> logs, JObject domainEventJObject, Subscription subscription, EventBusProvider provider)
        {
            if (provider != null)
            {

                var tokens = domainEventJObject.SelectTokens(subscription.JsonPathFilter);
                if (tokens.Any())
                {
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

                    var eventData = new Event() { Subscription = subscription, Payload = new EventPayload() { EventId = eventId, Arg = payload } };

                    var result = await provider.SendEventsAsync(new List<Event>() { eventData });

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        logs.Add(new ProviderConnectionLog() { ProviderName = subscription.ConnectionName, ErrorMessage = result.ErrorMessage, Status = result.Status });
                    }
                }
            }
        }

        protected virtual void InvokeHandler(Type eventType, IHandlerRegistrar registrar)
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


        protected virtual bool CheckEvents(IList<SubscriptionEventRequest> eventIds)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (eventIds.All(x => allEvents.Any(e => e.Id == x.EventId)))
            {
                return true;
            }
            else
            {
                var notRegisteredEvents = eventIds.Where(e => !allEvents.Any(all => all.Id == e.EventId)).Select(x => x.EventId);
                throw new PlatformException($"The events are not registered: {string.Join(",", notRegisteredEvents)}");
            }
        }
    }
}
