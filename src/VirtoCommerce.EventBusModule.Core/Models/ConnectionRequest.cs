namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class ProviderConnectionRequest
    {
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public string ConnectionOptionsSerialized { get; set; }

        public ProviderConnection ToModel()
        {
            return new ProviderConnection
            {
                Name = Name,
                ProviderName = ProviderName,
                ConnectionOptionsSerialized = ConnectionOptionsSerialized
            };
        }

    }
}
