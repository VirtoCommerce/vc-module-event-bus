using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class SubscriptionEventEntity : AuditableEntity, IDataEntity<SubscriptionEventEntity, SubscriptionEvent>
    {
        [StringLength(256)]
        public string EventId { get; set; }

        [StringLength(128)]
        public string SubscriptionId { get; set; }

        public virtual SubscriptionEntity Subscription { get; set; }

        public virtual SubscriptionEvent ToModel(SubscriptionEvent subscriptionEvent)
        {
            if (subscriptionEvent == null)
                throw new ArgumentNullException(nameof(subscriptionEvent));

            subscriptionEvent.Id = Id;
            subscriptionEvent.CreatedBy = CreatedBy;
            subscriptionEvent.CreatedDate = CreatedDate;
            subscriptionEvent.ModifiedBy = ModifiedBy;
            subscriptionEvent.ModifiedDate = ModifiedDate;
            subscriptionEvent.EventId = EventId;
            subscriptionEvent.SubscriptionId = SubscriptionId;

            return subscriptionEvent;
        }

        public virtual SubscriptionEventEntity FromModel(SubscriptionEvent subscriptionEvent, PrimaryKeyResolvingMap pkMap)
        {
            if (subscriptionEvent == null)
                throw new ArgumentNullException(nameof(subscriptionEvent));

            Id = subscriptionEvent.Id;
            CreatedBy = subscriptionEvent.CreatedBy;
            CreatedDate = subscriptionEvent.CreatedDate;
            ModifiedBy = subscriptionEvent.ModifiedBy;
            ModifiedDate = subscriptionEvent.ModifiedDate;
            EventId = subscriptionEvent.EventId;
            SubscriptionId = subscriptionEvent.SubscriptionId;

            pkMap.AddPair(subscriptionEvent, this);

            return this;
        }

        public virtual void Patch(SubscriptionEventEntity target)
        {
            target.EventId = EventId;
        }
    }
}
