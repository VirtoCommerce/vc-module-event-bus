namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class EventPayload
    {
        public string EventId { get; set; }
        public object Arg { get; set; }
    }
}
