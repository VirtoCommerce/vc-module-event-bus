using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class EventData
    {
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string EventId { get; set; }
        //public DomainEvent DomainEvent { get; set; }
    }
}
