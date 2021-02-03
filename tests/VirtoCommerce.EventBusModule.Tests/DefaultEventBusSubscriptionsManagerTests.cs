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
        public async Task SaveSubscriptionAsync_TrySaveKnownType_Saved()
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
        public async Task SaveSubscriptionAsync_TrySaveUnknownType_ThrowException()
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

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveKnownProvider_Saved()
        {
            //Arrange
            var eventBusManager = GetEventBusSubscriptionsManager();

            var request = new SubscriptionRequest() { Provider = "Known_Provider", EventIds = Array.Empty<string>() };

            //Act
            var result = await eventBusManager.SaveSubscriptionAsync(request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveUnknownProvider_ThrowException()
        {
            //Arrange
            var eventBusManager = GetEventBusSubscriptionsManager();

            var request = new SubscriptionRequest() { Provider = "Unknown_Provider", EventIds = Array.Empty<string>() };

            //Act
            var ex = await Assert.ThrowsAsync<PlatformException>(() => eventBusManager.SaveSubscriptionAsync(request));

            //Assert
            Assert.Equal(typeof(PlatformException), ex.GetType());
            Assert.Equal("The provider Unknown_Provider is not registered", ex.Message);
        }

        private DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IHandlerRegistrar handlerRegistrar, ISubscriptionService subscriptionService)
        {
            var registeredEventServiceMock = new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>());
            var eventTypes = new List<PlatformEventInfo>
            {
                new PlatformEventInfo { Id = typeof(FakeEvent).FullName, Type = typeof(FakeEvent) }
            };
            registeredEventServiceMock.Setup(x => x.GetAllEvents()).Returns(eventTypes);

            var eventBusProviderServiceMock = new Mock<IEventBusProviderService>();
            eventBusProviderServiceMock.Setup(x => x.IsProviderRegistered(It.IsAny<string>())).Returns(true);

            return new DefaultEventBusSubscriptionsManager(handlerRegistrar,
                registeredEventServiceMock.Object,
                subscriptionService,
                Mock.Of<ISubscriptionSearchService>(),
                eventBusProviderServiceMock.Object);
        }

        private DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager()
        {
            var eventBusProviderServiceMock = new Mock<IEventBusProviderService>();
            eventBusProviderServiceMock.Setup(x => x.IsProviderRegistered(It.Is<string>(x => x == "Known_Provider"))).Returns(true);

            return new DefaultEventBusSubscriptionsManager(new InProcessBus(),
                new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>()).Object,
                Mock.Of<ISubscriptionService>(),
                Mock.Of<ISubscriptionSearchService>(),
                eventBusProviderServiceMock.Object);
        }
    }

    class FakeEvent : DomainEvent { }
    class UnknownEvent { }
}
