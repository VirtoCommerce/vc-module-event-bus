using System;
using System.Runtime.Serialization;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class PlatformEventInfo
    {
        public string Id { get; set; }

        [IgnoreDataMember]
        public Type Type { get; set; }
    }
}
