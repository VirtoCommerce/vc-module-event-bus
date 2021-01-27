using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class SubscriptionEntity : AuditableEntity
    {
        [StringLength(128)]
        public string Provider { get; set; }
        [StringLength(512)]
        public string ConnectionString { get; set; }
        [StringLength(512)]
        public string AccessKey { get; set; }
        public int Status { get; set; }
        [StringLength(1024)]
        public string ErrorMessage { get; set; }
        public ObservableCollection<SubscriptionEventEntity> Events { get; set; }

        public virtual SubscriptionInfo ToModel(SubscriptionInfo subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            subscription.Id = Id;
            subscription.CreatedBy = CreatedBy;
            subscription.CreatedDate = CreatedDate;
            subscription.ModifiedBy = ModifiedBy;
            subscription.ModifiedDate = ModifiedDate;
            subscription.Provider = Provider;
            subscription.ConnectionString = ConnectionString;
            subscription.AccessKey = AccessKey;
            subscription.Status = Status;
            subscription.ErrorMessage = ErrorMessage;

            subscription.Events = Events.Select(x => x.ToModel(AbstractTypeFactory<SubscriptionEvent>.TryCreateInstance())).ToArray();

            return subscription;
        }

        public virtual SubscriptionEntity FromModel(SubscriptionInfo subscription, PrimaryKeyResolvingMap pkMap)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            Id = subscription.Id;
            CreatedBy = subscription.CreatedBy;
            CreatedDate = subscription.CreatedDate;
            ModifiedBy = subscription.ModifiedBy;
            ModifiedDate = subscription.ModifiedDate;
            Provider = subscription.Provider;
            ConnectionString = subscription.ConnectionString;
            AccessKey = subscription.AccessKey;
            Status = subscription.Status;
            ErrorMessage = subscription.ErrorMessage;

            if (subscription.Events != null)
            {
                Events = new ObservableCollection<SubscriptionEventEntity>(subscription.Events.Select(x => AbstractTypeFactory<SubscriptionEventEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            pkMap.AddPair(subscription, this);

            return this;
        }

        public virtual void Patch(SubscriptionEntity target)
        {
            target.Provider = Provider;
            target.ConnectionString = ConnectionString;
            target.AccessKey = AccessKey;
            target.Status = Status;
            target.ErrorMessage = ErrorMessage;

            if (!Events.IsNullCollection())
            {
                var eventComparer = AnonymousComparer.Create((SubscriptionEventEntity x) => $"{x.EventId}");
                Events.Patch(target.Events, eventComparer, (sourceEvent, targetEvent) => sourceEvent.Patch(targetEvent));
            }
        }
    }
}
