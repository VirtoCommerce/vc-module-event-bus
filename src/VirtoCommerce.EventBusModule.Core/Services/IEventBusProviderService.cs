using System;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusProviderService
    {
        void RegisterProvider<T>(string providerName, Func<T> factory = null) where T : EventBusProvider;

        EventBusProvider CreateProvider(string providerType);
    }
}
