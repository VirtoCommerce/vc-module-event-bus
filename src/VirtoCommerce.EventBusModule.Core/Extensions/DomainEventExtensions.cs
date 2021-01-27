using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Extensions
{
    public static class DomainEventExtensions
    {
        //Need to move the extension to the Platform
        public static TResult[] GetEntityWithInterface<TResult>(this IEvent obj)
        {
            var result = new List<TResult>();

            var objectType = obj.GetType();
            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var objects = properties.Where(p => (p.Name.Equals(nameof(GenericChangedEntryEvent<TResult>.ChangedEntries)) && p.GetIndexParameters().Length == 0
                                                || p.PropertyType.GetInterfaces().Contains(typeof(TResult))))
                                        .Select(x => x.GetValue(obj, null))
                                        .Where(x => !(x is string));

            foreach (var @object in objects)
            {
                if (@object is IEnumerable enumerable)
                {
                    foreach (var collectionObject in enumerable)
                    {
                        result.AddRange(collectionObject.GetType().GetProperties()
                                                      .Where(x => x.Name.EqualsInvariant(nameof(GenericChangedEntry<TResult>.NewEntry)))
                                                      .Select(p => p.GetValue(collectionObject))
                                                      .OfType<TResult>());
                    }
                }
                else if (@object is TResult concreteObject)
                {
                    result.Add(concreteObject);
                }
            }

            return result.ToArray();
        }
    }    
}
