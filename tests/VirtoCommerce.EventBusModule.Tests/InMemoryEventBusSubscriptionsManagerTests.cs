using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class InMemoryEventBusSubscriptionsManagerTests
    {

        [Fact]
        public async Task AddSubscription_KnownType_SubscribeAndGetEventFromSubscriptions()
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
            Assert.Contains(nameof(FakeEvent), subcriptions);
        }
        

        private InMemoryEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IHandlerRegistrar handlerRegistrar)
        {
            var registeredEventServiceMock = new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>());
            var eventTypes = new Dictionary<string, Type> { { nameof(FakeEvent), typeof(FakeEvent) } };
            registeredEventServiceMock.Setup(x => x.GetAllEvents()).Returns(eventTypes);

            return new InMemoryEventBusSubscriptionsManager(handlerRegistrar, registeredEventServiceMock.Object);
        }
    }

    class FakeEvent : DomainEvent { }
}
