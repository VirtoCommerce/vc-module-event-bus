using System;
using System.Linq;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusProviderService : IEventBusProviderService
    {
        public virtual void RegisterProvider<T>(string providerName, Func<T> factory = null) where T : EventBusProvider
        {
            if (AbstractTypeFactory<EventBusProvider>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                var typeInfo = AbstractTypeFactory<EventBusProvider>.RegisterType<T>();

                typeInfo.WithTypeName(providerName);

                if (factory != null)
                {
                    typeInfo.WithFactory(factory);
                }
            }
        }

        public virtual EventBusProvider CreateProvider(string providerName)
        {
            return AbstractTypeFactory<EventBusProvider>.TryCreateInstance(providerName, default(EventBusProvider));
        }
    }
}
