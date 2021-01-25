using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Core.Services
{
    public interface IEventBusSubscriptionsManager
    {
        void RegisterEvents();
        void AddSubscription<T>() where T : IEvent;
        void RemoveSubscription<T>() where T : IEvent;
        string[] GetEvents(int skip, int take);
        string[] GetEventSubscriptions(int skip, int take);
    }
}
