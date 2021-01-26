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
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class DefaultEventBusSubscriptionsManagerTests
    {

        [Fact]
        public async Task AddSubscription_KnownType_SubscribeAndGetEventFromSubscriptions()
        {
            //Arrange
            var eventBus = new InProcessBus();
            var eventBusManager = GetEventBusSubscriptionsManager(eventBus);
            eventBusManager.RegisterEvents();
            var request = new SubscriptionRequest() { EventIds = new[] { typeof(FakeEvent).FullName } };

            //Act
            var result = await eventBusManager.AddSubscriptionAsync(request);

            //Assert
            Assert.NotNull(result);
        }
        

        private DefaultEventBusSubscriptionsManager GetEventBusSubscriptionsManager(IHandlerRegistrar handlerRegistrar)
        {
            var registeredEventServiceMock = new Mock<RegisteredEventService>(Mock.Of<IPlatformMemoryCache>());
            var eventTypes = new Dictionary<string, Type> { { typeof(FakeEvent).FullName, typeof(FakeEvent) } };
            registeredEventServiceMock.Setup(x => x.GetAllEvents()).Returns(eventTypes);

            return new DefaultEventBusSubscriptionsManager(handlerRegistrar,
                registeredEventServiceMock.Object,
                Mock.Of<ISubscriptionService>(),
                Mock.Of<ISubscriptionSearchService>());
        }
    }

    class FakeEvent : DomainEvent { }
}
