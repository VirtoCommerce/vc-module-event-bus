using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class ProviderConnectionLogEntity : AuditableEntity, IDataEntity<ProviderConnectionLogEntity, ProviderConnectionLog>
    {
        public int Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorPayload { get; set; }

        [StringLength(128)]
        public string ProviderName { get; set; }

        public virtual ProviderConnectionLog ToModel(ProviderConnectionLog providerConnectionLog)
        {
            ArgumentNullException.ThrowIfNull(providerConnectionLog);

            providerConnectionLog.Id = Id;
            providerConnectionLog.CreatedBy = CreatedBy;
            providerConnectionLog.CreatedDate = CreatedDate;
            providerConnectionLog.ModifiedBy = ModifiedBy;
            providerConnectionLog.ModifiedDate = ModifiedDate;
            providerConnectionLog.Status = Status;
            providerConnectionLog.ErrorMessage = ErrorMessage;
            providerConnectionLog.ErrorPayload = ErrorPayload;
            providerConnectionLog.ProviderName = ProviderName;
            return providerConnectionLog;
        }

        public virtual ProviderConnectionLogEntity FromModel(ProviderConnectionLog providerConnectionLog, PrimaryKeyResolvingMap pkMap)
        {
            ArgumentNullException.ThrowIfNull(providerConnectionLog);

            Id = providerConnectionLog.Id;
            CreatedBy = providerConnectionLog.CreatedBy;
            CreatedDate = providerConnectionLog.CreatedDate;
            ModifiedBy = providerConnectionLog.ModifiedBy;
            ModifiedDate = providerConnectionLog.ModifiedDate;
            Status = providerConnectionLog.Status;
            ErrorMessage = providerConnectionLog.ErrorMessage;
            ErrorPayload = providerConnectionLog.ErrorPayload;
            ProviderName = providerConnectionLog.ProviderName;

            pkMap.AddPair(providerConnectionLog, this);

            return this;
        }

        public virtual void Patch(ProviderConnectionLogEntity target)
        {
            target.Status = Status;
            target.ErrorMessage = ErrorMessage;
            target.ErrorPayload = ErrorPayload;
            target.ProviderName = ProviderName;
        }
    }
}
