using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Data.Services;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests.IntegrationTests
{
    [Trait("Category", "IntegrationTest")]
    public class AzureEventBusProviderTests
    {
        private readonly AzureEventGridOptions _options;

        public AzureEventBusProviderTests()
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<AzureEventBusProviderTests>().Build();

            _options = new AzureEventGridOptions
            {
                ConnectionString = configuration["topicEndpoint"],
                AccessKey = configuration["topicAccessKey"],
            };
        }

        [Fact]
        public async Task AzureEventBusProvider_ShouldSucceed()
        {
            // Arrange
            var @event = new Event
            {
                Payload = new EventPayload
                {
                    EventId = "testEventId",
                    Arg = new
                    {
                        EventId = "testEventId",
                        ObjectId = "testObjectId",
                        ObjectType = "testObjectType",
                    },
                },
            };

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(_options));

            //Act
            var result = await azureProvider.SendEventsAsync([@event]);

            // Assert
            Assert.Equal(200, result.Status);
        }

        [Fact]
        public async Task AzureEventBusProvider_InvalidAccessKey_ShouldFail()
        {
            // Arrange
            var @event = new Event
            {
                Payload = new EventPayload
                {
                    EventId = "testEventId",
                    Arg = new
                    {
                        EventId = "testEventId",
                        ObjectId = "testObjectId",
                        ObjectType = "testObjectType",
                    },
                },
            };

            _options.AccessKey = "InvalidAccessKey";

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(_options));

            //Act
            var result = await azureProvider.SendEventsAsync([@event]);

            // Assert
            Assert.Equal(401, result.Status);
            Assert.NotNull(result.ErrorMessage);
            Assert.NotNull(result.ErrorPayload);
        }

        [Fact]
        public async Task AzureEventBusProvider_EmptyEventData_ShouldSucceed()
        {
            // Arrange
            var @event = new Event
            {
                Payload = new EventPayload
                {
                    EventId = "testEventId",
                },
            };

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(_options));

            //Act
            var result = await azureProvider.SendEventsAsync([@event]);

            // Assert
            Assert.Null(result.ErrorMessage);
        }
    }
}
