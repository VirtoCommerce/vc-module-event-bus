using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.EventBusModule.Data.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class RegisteredEventService
    {
        public readonly IPlatformMemoryCache _platformMemoryCache;

        public RegisteredEventService(IPlatformMemoryCache platformMemoryCache)
        {
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual Dictionary<string, Type> GetAllEvents()
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetAllEvents));
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(AllEventsCacheRegion.CreateChangeToken());
                return DiscoverAllDomainEvents();
            });
        }

        private static Dictionary<string, Type> DiscoverAllDomainEvents()
        {
            var eventType = typeof(IEvent);

            var result = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !(x.FullName.StartsWith("Microsoft.") || x.FullName.StartsWith("System.")))
                .SelectMany(x => GetTypesSafe(x))
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && x != typeof(DomainEvent) && x.GetInterfaces().Contains(eventType))
                .Distinct()
                .ToDictionary(k => k.FullName, v => v);
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
