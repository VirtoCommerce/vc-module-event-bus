using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly HashSet<Type> _subscriptions;
        private readonly IHandlerRegistrar _eventHandlerRegistrar;
        private readonly RegisteredEventService _registeredEventService;

        public InMemoryEventBusSubscriptionsManager(IHandlerRegistrar eventHandlerRegistrar, RegisteredEventService registeredEventService)
        {
            _subscriptions = new HashSet<Type>();
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _registeredEventService = registeredEventService;
        }

        #region Subcription
        
        public virtual void AddSubscription<T>() where T : IEvent
        {
            var eventType = typeof(T);

            if (!_subscriptions.Any(x => x == eventType))
            {
                _subscriptions.Add(eventType);
            }
        }

        public virtual void AddSubscription(string eventName)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (allEvents.TryGetValue(eventName, out var eventType) && !_subscriptions.Any(x => x == eventType))
            {
                _subscriptions.Add(eventType);
            }
        }

        public virtual string[] GetEvents(int skip, int take)
        {
            var allRegisteredEvents = _registeredEventService.GetAllEvents();
            return allRegisteredEvents.Skip(skip).Take(take).Select(x => x.Key).ToArray();
        }

        public virtual string[] GetEventSubscriptions(int skip, int take)
        {
            return _subscriptions.Skip(skip).Take(take).Select(x => x.FullName).ToArray();
        }

        public virtual void RemoveSubscription<T>() where T : IEvent
        {
            _subscriptions.Remove(typeof(T));
        }

        public virtual void RemoveSubscription(string eventName)
        {
            var allEvents = _registeredEventService.GetAllEvents();
            if (allEvents.TryGetValue(eventName, out var eventType) && !_subscriptions.Any(x => x == eventType))
            {
                _subscriptions.Remove(eventType);
            }
        }

        #endregion Subcription

        #region HandleEvent

        public virtual void RegisterEvents()
        {
            var allEvents = _registeredEventService.GetAllEvents();

            foreach (var @event in allEvents)
            {
                InvokeHandler(@event.Value, _eventHandlerRegistrar);
            }
        }


        protected virtual Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (_subscriptions.Any(x => x == domainEvent.GetType()))
            {
                //TODO call eventBusProvider
            }

            return Task.CompletedTask;
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
    }
}
