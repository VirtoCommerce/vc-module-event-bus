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
        private readonly IHandlerRegistrar _eventHandlerRegistrar;
        private readonly RegisteredEventService _registeredEventService;
        private readonly ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> _subscriptionSearchService;
        private readonly IEventBusProviderService _eventBusFactory;

        public DefaultEventBusSubscriptionsManager(IHandlerRegistrar eventHandlerRegistrar,
            RegisteredEventService registeredEventService,
            ICrudService<Subscription> subscriptionService,
            ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription> subscriptionSearchService,
            IEventBusProviderService eventBusFactory)
        {
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _registeredEventService = registeredEventService;
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _eventBusFactory = eventBusFactory;
        }

        #region Subcription

        public virtual async Task<Subscription> SaveSubscriptionAsync(SubscriptionRequest request)
        {
            Subscription result = null;
            if (CheckEvents(request.EventIds) &&
                CheckProviderConnection(request.ConnectionName))
            {
                result = request.ToModel();
                await _subscriptionService.SaveChangesAsync(new[] { result });
            }

            return result;
        }

        #endregion Subcription

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
            var criteria = new SubscriptionSearchCriteria()
            {
                EventIds = new[] { eventId },
                Skip = 0,
                Take = int.MaxValue,
            };
            var searchResult = await _subscriptionSearchService.SearchAsync(criteria);

            if (searchResult.TotalCount > 0)
            {
                /*
                var entities = domainEvent.GetObjectsWithDerived<IEntity>()
                                             .Select(x => new EventData { ObjectId = x.Id, ObjectType = x.GetType().FullName, EventId = eventId})
                                             .ToArray();
                var valueObjects = domainEvent.GetObjectsWithDerived<ValueObject>()
                                             .Select(x => new EventData { ObjectId = x.GetCacheKey(), ObjectType = x.GetType().FullName, EventId = eventId})
                                             .ToArray();
                var eventData = entities.Union(valueObjects).ToArray();
                */


                //var activeSubscritions = new List<SubscriptionInfo>();

                var domainEventJson = JsonConvert.SerializeObject(domainEvent);
                var domainEventJObject = JObject.Parse(domainEventJson);

                foreach (var subscription in searchResult.Results)
                {
                    var provider = _eventBusFactory.CreateProvider(subscription.ConnectionName);

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

                            var result = await provider.SendEventAsync(eventData);

                            /*
                            subscription.Status = result.Status;
                            subscription.ErrorMessage = string.Empty;

                            if (!string.IsNullOrEmpty(result.ErrorMessage))
                            {
                                subscription.ErrorMessage = new string(result.ErrorMessage.Take(1024).ToArray());
                            }


                            activeSubscritions.Add(subscription);
                            */
                        }
                    }
                }

                //await _subscriptionService.SaveChangesAsync(activeSubscritions.ToArray());
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


        private bool CheckEvents(string[] eventIds)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (eventIds.All(x => allEvents.Any(e => e.Id == x)))
            {
                return true;
            }
            else
            {
                var notRegisteredEvents = eventIds.Where(e => !allEvents.Any(all => all.Id == e));
                throw new PlatformException($"The events are not registered: {string.Join(",", notRegisteredEvents)}");
            }
        }

        private bool CheckProvider(string providerName)
        {
            if (_eventBusFactory.IsProviderRegistered(providerName))
            {
                return true;
            }
            else
            {
                throw new PlatformException($"The provider {providerName} is not registered");
            }
        }

        private bool CheckProviderConnection(string connectionName)
        {
            // TODO: add call for service
            return true;
        }
    }
}
