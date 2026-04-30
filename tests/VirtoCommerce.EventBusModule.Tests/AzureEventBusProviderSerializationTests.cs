using System.Collections.Generic;
using Azure.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Data.Services;
using Xunit;

namespace VirtoCommerce.EventBusModule.Tests
{
    public class AzureEventBusProviderSerializationTests
    {
        private const string RenderedJson =
            @"{""productId"":""abc"",""fulfillmentCenterId"":""ffc-1"",""status"":""LowStock"",""quantity"":42,""nested"":{""inner"":""value""}}";

        // Reproduces the templated path from DefaultEventBusSubscriptionsManager.SendEvent —
        // a JObject (the result of JsonConvert.DeserializeObject(scribanRender)) is fed in as
        // EventPayload.Arg with a non-empty PayloadTransformationTemplate, then the full
        // BuildCloudEvents loop is executed exactly as production runs it. Without the fix
        // (i.e. before the JToken branch in BuildCloudEvent) this test fails: every primitive
        // serializes as [] because Azure.Messaging.CloudEvent serializes `data` with STJ,
        // which has no converter for Newtonsoft's JToken/JValue.
        [Fact]
        public void BuildCloudEvents_TemplatedJObjectPayload_PreservesPrimitives()
        {
            var provider = new TestableProvider();
            var jobject = (JObject)JsonConvert.DeserializeObject(RenderedJson)!;

            var cloudEvents = provider.Build(new[]
            {
                new Event
                {
                    Subscription = new Subscription
                    {
                        Name = "test-sub",
                        PayloadTransformationTemplate = "{ \"productId\": \"{{ ProductId }}\" }",
                    },
                    Payload = new EventPayload { EventId = "test-event", Arg = jobject },
                },
            });

            var ce = Assert.Single(cloudEvents);
            var serialized = ce.Data!.ToString();

            Assert.DoesNotContain("[]", serialized);
            Assert.Contains("\"productId\":\"abc\"", serialized);
            Assert.Contains("\"fulfillmentCenterId\":\"ffc-1\"", serialized);
            Assert.Contains("\"status\":\"LowStock\"", serialized);
            Assert.Contains("\"quantity\":42", serialized);
            Assert.Contains("\"nested\":{\"inner\":\"value\"}", serialized);
            Assert.Equal("application/json", ce.DataContentType);
        }

        [Fact]
        public void BuildCloudEvents_TemplatedJArrayPayload_PreservesValues()
        {
            var provider = new TestableProvider();
            var jarray = (JArray)JsonConvert.DeserializeObject(@"[""one"",""two"",3]")!;

            var cloudEvents = provider.Build(new[]
            {
                new Event
                {
                    Subscription = new Subscription
                    {
                        Name = "test-sub",
                        PayloadTransformationTemplate = "[ ... ]",
                    },
                    Payload = new EventPayload { EventId = "test-event", Arg = jarray },
                },
            });

            var ce = Assert.Single(cloudEvents);
            Assert.Equal("application/json", ce.DataContentType);
            Assert.Equal(@"[""one"",""two"",3]", ce.Data!.ToString());
        }

        // Default-template path (no PayloadTransformationTemplate, payload is a plain CLR object)
        // must keep using the object overload of CloudEvent and serialize via STJ as before.
        [Fact]
        public void BuildCloudEvents_NonTemplatedPlainObjectPayload_UsesObjectOverload()
        {
            var provider = new TestableProvider();
            var record = new { EventId = "evt-1", ObjectId = "obj-1", ObjectType = "TestType" };

            var cloudEvents = provider.Build(new[]
            {
                new Event
                {
                    Subscription = new Subscription { Name = "test-sub" }, // no template
                    Payload = new EventPayload { EventId = "evt-1", Arg = record },
                },
            });

            var ce = Assert.Single(cloudEvents);
            var serialized = ce.Data!.ToString();
            Assert.Contains("\"EventId\":\"evt-1\"", serialized);
            Assert.Contains("\"ObjectId\":\"obj-1\"", serialized);
            Assert.Contains("\"ObjectType\":\"TestType\"", serialized);
        }

        private sealed class TestableProvider : AzureEventBusProvider
        {
            public List<CloudEvent> Build(IEnumerable<Event> events) => BuildCloudEvents(events);
        }
    }
}
