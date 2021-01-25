using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class InMemoryEventBusSubscriptionsManagerTests
    {

        [Fact]
        public async Task AddSubscription_SubscribeAndGetEventSubscriptions()
        {
            //Arrange
            var eventBus = new InProcessBus();
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus);
            eventBusManager.RegisterEvents();
            eventBusManager.AddSubscription<FakeEvent>();

            //Act
            await eventBus.Publish(new FakeEvent());
            var subcriptions = eventBusManager.GetEventSubscriptions(0, int.MaxValue);

            //Assert
            Assert.NotNull(subcriptions);
        }
        

        private InMemoryEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IHandlerRegistrar handlerRegistrar)
        {
            return new InMemoryEventBusSubscriptionsManager(handlerRegistrar);
        }
    }

    class FakeEvent : DomainEvent { }


}
