using Microsoft.Extensions.Configuration;
using VirtoCommerce.EventBusModule.Data.Services;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests.IntegrationTests
{
    public class AzureEventBusProviderTests
    {
        public IConfiguration Configuration { get; set; }

        public AzureEventBusProviderTests()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<AzureEventBusProviderTests>();

            Configuration = builder.Build();
        }

        [Fact]
        public async void AzureEventBusProviderSendTest()
        {
            var topicEndpoint = Configuration["topicEndpoint"];
            var topicAccessKey = Configuration["topicAccessKey"];

            var provider = new AzureEventBusProvider();

            var result = await provider.SendEventAsync();

            Assert.Equal("OK", result.ResponseResult);
        }
    }
}
