namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class Event
    {
        public Subscription Subscription { get; set; }
        public EventPayload Payload { get; set; }
    }
}
