using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class RegisteredEventServiceTests
    {

        [Fact]
        public void GetAllEvents()
        {
            //Arrange
            var service = GetRegisteredEventService();

            //Act
            var events = service.GetAllEvents();

            //Assert
            Assert.Contains(typeof(FakeEvent), events.Values);
            Assert.DoesNotContain(typeof(DomainEvent), events.Values);
        }


        private RegisteredEventService GetRegisteredEventService()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var platformMemoryCache = new PlatformMemoryCache(memoryCache, Options.Create(new CachingOptions()), new Mock<ILogger<PlatformMemoryCache>>().Object);
            return new RegisteredEventService(platformMemoryCache);
        }
    }
}
