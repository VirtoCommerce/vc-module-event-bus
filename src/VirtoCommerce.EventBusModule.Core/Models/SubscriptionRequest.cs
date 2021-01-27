namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class SubscriptionRequest
    {
        public string SubscriptionId { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string[] EventIds { get; set; }
    }
}
