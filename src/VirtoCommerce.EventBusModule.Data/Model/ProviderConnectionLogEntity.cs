using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class ProviderConnectionLogEntity :AuditableEntity, IDataEntity<ProviderConnectionLogEntity, ProviderConnectionLog>
    {
        public int Status { get; set; }
        public string ErrorMessage { get; set; }

        public virtual ProviderConnectionLog ToModel(ProviderConnectionLog providerConnectionLog)
        {
            if (providerConnectionLog == null)
                throw new ArgumentNullException(nameof(providerConnectionLog));

            providerConnectionLog.Id = Id;
            providerConnectionLog.CreatedBy = CreatedBy;
            providerConnectionLog.CreatedDate = CreatedDate;
            providerConnectionLog.ModifiedBy = ModifiedBy;
            providerConnectionLog.ModifiedDate = ModifiedDate;
            providerConnectionLog.Status = Status;
            providerConnectionLog.ErrorMessage = ErrorMessage;
            return providerConnectionLog;
        }

        public virtual ProviderConnectionLogEntity FromModel(ProviderConnectionLog providerConnectionLog, PrimaryKeyResolvingMap pkMap)
        {
            if (providerConnectionLog == null)
                throw new ArgumentNullException(nameof(providerConnectionLog));

            Id = providerConnectionLog.Id;
            CreatedBy = providerConnectionLog.CreatedBy;
            CreatedDate = providerConnectionLog.CreatedDate;
            ModifiedBy = providerConnectionLog.ModifiedBy;
            ModifiedDate = providerConnectionLog.ModifiedDate;
            Status = providerConnectionLog.Status;
            ErrorMessage = providerConnectionLog.ErrorMessage;

            pkMap.AddPair(providerConnectionLog, this);

            return this;
        }

        public virtual void Patch(ProviderConnectionLogEntity target)
        {
            target.Status = Status;
            target.ErrorMessage = ErrorMessage;
        }
    }
}
