using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.EventBusModule.Core.Models
{
    public class EventPayload
    {        
        public string EventId { get; set; }
        public object Arg { get; set; }
    }
}
