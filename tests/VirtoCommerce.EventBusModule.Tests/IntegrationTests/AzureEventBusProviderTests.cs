using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Data.Services;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests.IntegrationTests
{
    [Trait("Category", "IntegrationTest")]
    public class AzureEventBusProviderTests
    {
        public IConfiguration Configuration { get; set; }

        private string _topicEndpoint;
        private string _topicAccessKey;

        public AzureEventBusProviderTests()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<AzureEventBusProviderTests>();

            Configuration = builder.Build();

            _topicEndpoint = Configuration["topicEndpoint"];
            _topicAccessKey = Configuration["topicAccessKey"];
        }

        [Fact]
        public async void AzureEventBusProviderSendTest()
        {
            // Arrange
            var subscription = new SubscriptionInfo()
            {
                Id = "testSubscription",
                Provider = "AzureEventGrid",
                ConnectionString = _topicEndpoint,
                AccessKey = _topicAccessKey,
            };

            var eventData = new EventData()
            {
                ObjectId = Guid.NewGuid().ToString(),
                ObjectType = "testObjectType",
                EventId = "testEventId",
            };

            //Act
            var result = await new AzureEventBusProvider().SendEventAsync(subscription, new List<EventData>() { eventData });

            // Assert
            Assert.Equal(200, result.Status);
        }

        [Fact]
        public async void AzureEventBusProviderSendErrorTest()
        {
            // Arrange
            var subscription = new SubscriptionInfo()
            {
                Id = "testSubscription",
                Provider = "AzureEventGrid",
                ConnectionString = _topicEndpoint,
                AccessKey = "gibberish",
            };

            var eventData = new EventData()
            {
                ObjectId = Guid.NewGuid().ToString(),
                ObjectType = "testObjectType",
                EventId = "testEventId",
            };

            // Act
            var result = await new AzureEventBusProvider().SendEventAsync(subscription, new List<EventData>() { eventData });

            // Assert
            Assert.Equal(401, result.Status);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async void AzureEventBusProviderEmptyCredentials()
        {
            // Arrange
            var subscription = new SubscriptionInfo()
            {
                Id = "testSubscription",
                Provider = "AzureEventGrid",
            };

            // Act
            var result = await new AzureEventBusProvider().SendEventAsync(subscription, new List<EventData>());

            // Assert
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
