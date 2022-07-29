using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class SubscriptionEntity : AuditableEntity, IDataEntity<SubscriptionEntity, Subscription>
    {
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(128)]
        public string ConnectionName { get; set; }
        [StringLength(512)]
        public string JsonPathFilter { get; set; }
        public string PayloadTransformationTemplate { get; set; }
        public string EventSettingsSerialized { get; set; }

        public ObservableCollection<SubscriptionEventEntity> Events { get; set; }

        public virtual Subscription ToModel(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            subscription.Id = Id; 
            subscription.CreatedBy = CreatedBy;
            subscription.CreatedDate = CreatedDate;
            subscription.ModifiedBy = ModifiedBy;
            subscription.ModifiedDate = ModifiedDate;
            subscription.Name = Name;
            subscription.ConnectionName = ConnectionName;
            subscription.JsonPathFilter = JsonPathFilter;
            subscription.PayloadTransformationTemplate = PayloadTransformationTemplate;
            subscription.EventSettingsSerialized = EventSettingsSerialized;

            subscription.Events = Events.Select(x => x.ToModel(AbstractTypeFactory<SubscriptionEvent>.TryCreateInstance())).ToArray();

            return subscription;
        }

        public virtual SubscriptionEntity FromModel(Subscription subscription, PrimaryKeyResolvingMap pkMap)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            Id = subscription.Id;
            CreatedBy = subscription.CreatedBy;
            CreatedDate = subscription.CreatedDate;
            ModifiedBy = subscription.ModifiedBy;
            ModifiedDate = subscription.ModifiedDate;
            Name = subscription.Name;
            ConnectionName = subscription.ConnectionName;
            JsonPathFilter = subscription.JsonPathFilter;
            PayloadTransformationTemplate = subscription.PayloadTransformationTemplate;
            EventSettingsSerialized = subscription.EventSettingsSerialized;

            if (subscription.Events != null)
            {
                Events = new ObservableCollection<SubscriptionEventEntity>(subscription.Events.Select(x => AbstractTypeFactory<SubscriptionEventEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            pkMap.AddPair(subscription, this);

            return this;
        }

        public virtual void Patch(SubscriptionEntity target)
        {
            target.Name = Name;
            target.ConnectionName = ConnectionName;
            target.JsonPathFilter = JsonPathFilter;
            target.PayloadTransformationTemplate = PayloadTransformationTemplate;
            target.EventSettingsSerialized = EventSettingsSerialized;

            if (!Events.IsNullCollection())
            {
                var eventComparer = AnonymousComparer.Create((SubscriptionEventEntity x) => $"{x.EventId}");
                Events.Patch(target.Events, eventComparer, (sourceEvent, targetEvent) => sourceEvent.Patch(targetEvent));
            }
        }
    }
}
