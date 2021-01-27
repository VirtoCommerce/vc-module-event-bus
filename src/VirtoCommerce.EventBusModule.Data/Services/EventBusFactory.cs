using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class EventBusFactory : IEventBusFactory
    {
        public virtual IEventBusProvider CreateProvider(string providerType)
        {
            var provider = default(IEventBusProvider);

            if (string.IsNullOrEmpty(providerType) || providerType.EqualsInvariant("AzureEventGrid"))
            {
                provider = AbstractTypeFactory<AzureEventBusProvider>.TryCreateInstance();
            }

            return provider;
        }
    }
}
