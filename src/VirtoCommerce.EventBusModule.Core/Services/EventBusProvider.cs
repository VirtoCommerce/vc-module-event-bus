using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Models;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public abstract class EventBusProvider
    {
        public abstract Task<SendEventResult> SendEventsAsync(IEnumerable<Event> events);
        public abstract bool IsConnected();
        public abstract void SetConnectionOptions(JObject options);
        public abstract bool Connect();
    }
}
