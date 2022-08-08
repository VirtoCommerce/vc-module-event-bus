using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.EventBusModule.Data.Model
{
    public class ProviderConnectionEntity : AuditableEntity, IDataEntity<ProviderConnectionEntity, ProviderConnection>
    {
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(128)]
        public string ProviderName { get; set; }
        public string ConnectionOptionsSerialized { get; set; }


        public virtual ProviderConnection ToModel(ProviderConnection providerConnection)
        {
            if (providerConnection == null)
                throw new ArgumentNullException(nameof(providerConnection));

            providerConnection.Id = Id;
            providerConnection.CreatedBy = CreatedBy;
            providerConnection.CreatedDate = CreatedDate;
            providerConnection.ModifiedBy = ModifiedBy;
            providerConnection.ModifiedDate = ModifiedDate;
            providerConnection.Name = Name;
            providerConnection.ProviderName = ProviderName;
            providerConnection.ConnectionOptionsSerialized = ConnectionOptionsSerialized;
            return providerConnection;
        }

        public virtual ProviderConnectionEntity FromModel(ProviderConnection providerConnection, PrimaryKeyResolvingMap pkMap)
        {
            if (providerConnection == null)
                throw new ArgumentNullException(nameof(providerConnection));

            Id = providerConnection.Id;
            CreatedBy = providerConnection.CreatedBy;
            CreatedDate = providerConnection.CreatedDate;
            ModifiedBy = providerConnection.ModifiedBy;
            ModifiedDate = providerConnection.ModifiedDate;
            Name = providerConnection.Name;
            ProviderName = providerConnection.ProviderName;
            ConnectionOptionsSerialized = providerConnection.ConnectionOptionsSerialized;

            pkMap.AddPair(providerConnection, this);

            return this;
        }

        public virtual void Patch(ProviderConnectionEntity target)
        {
            target.Name = Name;
            target.ProviderName = ProviderName;
            target.ConnectionOptionsSerialized = ConnectionOptionsSerialized;
        }
    }
}
