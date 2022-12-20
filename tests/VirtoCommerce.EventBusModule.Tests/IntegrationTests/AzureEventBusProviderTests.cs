using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        public IConfiguration Configuration { get; set; }

        private AzureEventGridOptions options;

        public AzureEventBusProviderTests()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<AzureEventBusProviderTests>();

            Configuration = builder.Build();

            options = new AzureEventGridOptions()
            {
                ConnectionString = Configuration["topicEndpoint"],
                AccessKey = Configuration["topicAccessKey"]
            };
        }

        [Fact]
        public async Task AzureEventBusProviderSendTest()
        {
            // Arrange
            var subscription = new Subscription()
            {
                Id = "testSubscription",                
                ConnectionName = "AzureEventGrid",
            };

            var eventData = new Event()
            {
                Subscription = subscription,
                Payload = new EventPayload() { EventId = "testEventId", Arg = new
                {
                    ObjectId = Guid.NewGuid().ToString(),
                    ObjectType = "testObjectType",
                    EventId = "testEventId",
                } }
            };

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(options));
            //Act
            var result = await azureProvider.SendEventsAsync(new List<Event>() { eventData });

            // Assert
            Assert.Equal(200, result.Status);
        }

        [Fact]
        public async Task AzureEventBusProviderSendErrorTest()
        {
            // Arrange
            var subscription = new Subscription()
            {
                Id = "testSubscription",
                ConnectionName = "AzureEventGrid",
            };

            var eventData = new Event()
            {
                Subscription = subscription,
                Payload = new EventPayload() { EventId = "testEventId", Arg = new
                {
                    ObjectId = Guid.NewGuid().ToString(),
                    ObjectType = "testObjectType",
                    EventId = "testEventId",
                } }
            };

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(options));
            //Act
            var result = await azureProvider.SendEventsAsync(new List<Event>() { eventData });

            // Assert
            Assert.Equal(401, result.Status);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async Task AzureEventBusProviderEmptyCredentials()
        {
            // Arrange
            var subscription = new Subscription()
            {
                Id = "testSubscription",
                ConnectionName = "AzureEventGrid",
            };

            var eventData = new Event()
            {
                Subscription = subscription,
                Payload = new EventPayload() { EventId = "testEventId" }
            };

            var azureProvider = new AzureEventBusProvider();
            azureProvider.SetConnectionOptions(JObject.FromObject(options));
            //Act
            var result = await azureProvider.SendEventsAsync(new List<Event>() { eventData });

            // Assert
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
