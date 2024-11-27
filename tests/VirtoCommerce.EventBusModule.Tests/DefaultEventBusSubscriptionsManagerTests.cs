using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
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

        public static ILogger<T> GetLogger<T>()
        {
            return new Mock<ILogger<T>>().Object;
        }

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveKnownType_Saved()
        {
            //Arrange
            var subscriptionServiceMock = new Mock<ISubscriptionService>();

            var eventBus = new InProcessBus(GetLogger<InProcessBus>());
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus, subscriptionServiceMock.Object);
            eventBusManager.RegisterEvents();
            var request = new SubscriptionRequest() { ConnectionName = "FakeProvider", Events = new List<SubscriptionEventRequest>() { new SubscriptionEventRequest() { EventId = typeof(FakeEvent).FullName } } };

            //Act
            var result = await eventBusManager.SaveSubscriptionAsync(request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SaveSubscriptionAsync_TrySaveUnknownType_ThrowException()
        {
            //Arrange
            var subscriptionServiceMock = new Mock<ISubscriptionService>();

            var eventBus = new InProcessBus(GetLogger<InProcessBus>());
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus, subscriptionServiceMock.Object);
            eventBusManager.RegisterEvents();
            var request = new SubscriptionRequest() { ConnectionName = "FakeProvider", Events = new List<SubscriptionEventRequest>() { new SubscriptionEventRequest() { EventId = typeof(UnknownEvent).FullName } } };

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

            var request = new SubscriptionRequest() { ConnectionName = "Known_Provider", Events = new List<SubscriptionEventRequest>() };

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

            var request = new SubscriptionRequest() { ConnectionName = "Unknown_Provider", Events = new List<SubscriptionEventRequest>() };

            //Act
            var ex = await Assert.ThrowsAsync<PlatformException>(async () => await eventBusManager.SaveSubscriptionAsync(request));

            //Assert
            Assert.Equal(typeof(PlatformException), ex.GetType());
            Assert.Equal("The provider connection Unknown_Provider is not registered", ex.Message);
        }

        private static DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IEventHandlerRegistrar eventHandlerRegistrar, ISubscriptionService subscriptionService)
        {
            var registeredEventServiceMock = new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>());
            var eventTypes = new List<PlatformEventInfo>
            {
                new PlatformEventInfo { Id = typeof(FakeEvent).FullName, Type = typeof(FakeEvent) }
            };
            registeredEventServiceMock.Setup(x => x.GetAllEvents()).Returns(eventTypes);

            var eventBusProviderServiceMock = new Mock<IEventBusProviderConnectionsService>();
            eventBusProviderServiceMock.Setup(x => x.GetProviderConnectionAsync(It.Is<string>(x => x == "FakeProvider"))).ReturnsAsync(new ProviderConnection());
            eventBusProviderServiceMock.Setup(x => x.GetConnectedProviderAsync(It.IsAny<string>())).ReturnsAsync(new FakeProvider());

            return new DefaultEventBusSubscriptionsManager(eventHandlerRegistrar,
                registeredEventServiceMock.Object,
                subscriptionService,
                Mock.Of<IProviderConnectionLogService>(),
                Mock.Of<IEventBusSubscriptionsService>(),
                eventBusProviderServiceMock.Object,
                Mock.Of<ILogger<DefaultEventBusSubscriptionsManager>>());
        }

        private static DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager()
        {

            var providerConnectionSearchServiceMock = new Mock<IProviderConnectionSearchService>();
            providerConnectionSearchServiceMock
                .Setup(x => x.SearchAsync(It.Is<ProviderConnectionSearchCriteria>(x => x.Name == "Known_Provider"), It.IsAny<bool>()))
                .ReturnsAsync(
                    new ProviderConnectionSearchResult()
                    {
                        TotalCount = 1,
                        Results = new List<ProviderConnection>() { new ProviderConnection() { ProviderName = "Known_Provider" } }
                    });

            var eventBusProviderConnectionsService = new EventBusProviderConnectionsService(Mock.Of<IEventBusProviderService>(), providerConnectionSearchServiceMock.Object, Mock.Of<IEventBusReadConfigurationService>());

            return new DefaultEventBusSubscriptionsManager(new InProcessBus(GetLogger<InProcessBus>()),
                new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>()).Object,
                Mock.Of<ISubscriptionService>(),
                Mock.Of<IProviderConnectionLogService>(),
                Mock.Of<IEventBusSubscriptionsService>(),
                eventBusProviderConnectionsService,
                Mock.Of<ILogger<DefaultEventBusSubscriptionsManager>>());
        }
    }

    class FakeEvent : DomainEvent { }
    class UnknownEvent { }

    class FakeProvider : EventBusProvider
    {
        public override bool Connect()
        {
            return true;
        }

        public override bool IsConnected()
        {
            return true;
        }

        public override Task<SendEventResult> SendEventsAsync(IEnumerable<Event> events)
        {
            return Task.FromResult(new SendEventResult());
        }

        public override void SetConnectionOptions(JObject options)
        {
        }
    }
}
