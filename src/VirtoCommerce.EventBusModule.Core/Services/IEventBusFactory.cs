namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusFactory
    {
        IEventBusProvider CreateProvider(string providerType);
    }
}
