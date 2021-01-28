using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class DefaultEventBusSubscriptionsManagerTests
    {

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveKnownType()
        {
            //Arrange
            var subcriptionServiceMock = new Mock<ISubscriptionService>();
            
            var eventBus = new InProcessBus();
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus, subcriptionServiceMock.Object);
            eventBusManager.RegisterEvents();
            var request = new SubscriptionRequest() { EventIds = new[] { typeof(FakeEvent).FullName } };

            //Act
            var result = await eventBusManager.SaveSubscriptionAsync(request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveUnknownType()
        {
            //Arrange
            var subcriptionServiceMock = new Mock<ISubscriptionService>();

            var eventBus = new InProcessBus();
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus, subcriptionServiceMock.Object);
            eventBusManager.RegisterEvents();
            var request = new SubscriptionRequest() { EventIds = new[] { typeof(UnknownEvent).FullName } };

            //Act
            var ex = await Assert.ThrowsAsync<PlatformException>(() => eventBusManager.SaveSubscriptionAsync(request));

            //Assert
            Assert.Equal(typeof(PlatformException), ex.GetType());
            Assert.Equal("The events are not registered: VirtoCommerce.EventBusModule.Tests.UnknownEvent", ex.Message);
        }


        private DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IHandlerRegistrar handlerRegistrar, ISubscriptionService subscriptionService)
        {
            var registeredEventServiceMock = new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>());
            var eventTypes = new Dictionary<string, Type> { { typeof(FakeEvent).FullName, typeof(FakeEvent) } };
            registeredEventServiceMock.Setup(x => x.GetAllEvents()).Returns(eventTypes);

            return new DefaultEventBusSubscriptionsManager(handlerRegistrar,
                registeredEventServiceMock.Object,
                subscriptionService,
                Mock.Of<ISubscriptionSearchService>());
        }
    }

    class FakeEvent : DomainEvent { }
    class UnknownEvent { }
}
