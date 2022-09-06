using System;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusProviderService
    {
        void RegisterProvider<T>(string providerName, Func<T> factory = null) where T : EventBusProvider;

        /// <summary>
        /// Instantiate provider with specific name
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        EventBusProvider CreateProvider(string providerName);

        bool IsProviderRegistered(string providerName);
    }
}
