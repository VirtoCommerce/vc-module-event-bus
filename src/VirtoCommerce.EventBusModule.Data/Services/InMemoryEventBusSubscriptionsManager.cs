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

        public InMemoryEventBusSubscriptionsManager(IHandlerRegistrar eventHandlerRegistrar)
        {
            _subscriptions = new HashSet<Type>();
            _eventHandlerRegistrar = eventHandlerRegistrar;
        }

        public virtual void RegisterEvents()
        {
            var allRegisteredEvents = DiscoverAllDomainEvents();

            foreach (var registeredEvent in allRegisteredEvents)
            {
                InvokeHandler(registeredEvent, _eventHandlerRegistrar);
            }
        }

        public virtual void AddSubscription<T>() where T : IEvent
        {
            var eventType = typeof(T);

            if (!_subscriptions.Any(x => x == eventType))
            {
                _subscriptions.Add(eventType);
            }
        }

        public virtual string[] GetEvents(int skip, int take)
        {
            var allRegisteredEvents = DiscoverAllDomainEvents();
            return allRegisteredEvents.Skip(skip).Take(take).Select(x => x.FullName).ToArray();
        }

        public virtual string[] GetEventSubscriptions(int skip, int take)
        {
            return _subscriptions.Skip(skip).Take(take).Select(x => x.FullName).ToArray();
        }

        public virtual void RemoveSubscription<T>() where T : IEvent
        {
            _subscriptions.Remove(typeof(T));
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

        private static Type[] DiscoverAllDomainEvents()
        {
            var eventBaseType = typeof(DomainEvent);

            var result = AppDomain.CurrentDomain.GetAssemblies()
                // Maybe there is a way to find platform- and modules- related assemblies
                .Where(x => !(x.FullName.StartsWith("Microsoft.") || x.FullName.StartsWith("System.")))
                .SelectMany(x => GetTypesSafe(x))
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && x.IsSubclassOf(eventBaseType))
                .Distinct()
                .ToArray();
            return result;
        }

        private static Type[] GetTypesSafe(Assembly assembly)
        {
            var result = Array.Empty<Type>();

            try
            {
                result = assembly.GetTypes();
            }
            catch (Exception ex) when (ex is ReflectionTypeLoadException || ex is TypeLoadException)
            {
                // No need to trow as we could have exceptions when loading types
            }

            return result;
        }
    }
}
