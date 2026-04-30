using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Extensions;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class AzureEventBusProvider : EventBusProvider
    {
        private EventGridPublisherClient _client;

        public override bool Connect()
        {
            return true;
        }

        public override bool IsConnected()
        {
            return _client != null;
        }

        public override async Task<SendEventResult> SendEventsAsync(IEnumerable<Event> events)
        {
            var result = new SendEventResult();
            var cloudEvents = new List<CloudEvent>();

            try
            {
                cloudEvents = BuildCloudEvents(events);

                var eventGridResponse = await _client.SendEventsAsync(cloudEvents);

                result.Status = eventGridResponse.Status;
            }
            catch (Exception ex)
            {
                result.Status = ex is RequestFailedException requestFailedEx
                    ? requestFailedEx.Status
                    : StatusCodes.Status500InternalServerError;

                result.ErrorMessage = ex.Message;
                result.ErrorPayload = JsonConvert.SerializeObject(cloudEvents);
            }

            return result;
        }

        protected virtual List<CloudEvent> BuildCloudEvents(IEnumerable<Event> events)
        {
            var cloudEvents = new List<CloudEvent>();

            foreach (var @event in events)
            {
                // Currently, AzureEventBusProvider does not have azure-specific event translation settings.
                // Here we can add those in the future if needed.
                // To allow this you should make a descendant from ProviderSpecificEventSettings,
                // then add deserialization from @event.Subscription.EventSettings

                var subscription = @event.Subscription;
                var subscriptionName = subscription?.Name ?? nameof(AzureEventBusProvider);
                var eventId = @event.Payload.EventId;
                var eventData = @event.Payload.Arg;

                var eventDataItems = new List<object>();

                if (string.IsNullOrEmpty(subscription?.PayloadTransformationTemplate) && eventData is IEvent nativeEvent)
                {
                    // Implement default behavior equal to previous version of event bus module (compatibility)

                    // Entities
                    eventDataItems.AddRange(nativeEvent
                        .GetObjectsWithDerived<IEntity>()
                        .Select(x => new
                        {
                            EventId = eventId,
                            ObjectId = x.Id,
                            ObjectType = x.GetType().FullName,
                        }));

                    // Value objects
                    eventDataItems.AddRange(nativeEvent
                        .GetObjectsWithDerived<ValueObject>()
                        .Select(x => new
                        {
                            EventId = eventId,
                            ObjectId = x.GetCacheKey(),
                            ObjectType = x.GetType().FullName,
                        }));
                }

                if (eventDataItems.Count == 0)
                {
                    eventDataItems.Add(eventData);
                }

                cloudEvents.AddRange(eventDataItems.Select(x => BuildCloudEvent(subscriptionName, eventId, x)));
            }

            return cloudEvents;
        }

        public override void SetConnectionOptions(JObject options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var evtGridOptions = options.ToObject<AzureEventGridOptions>();

            _client = new EventGridPublisherClient(new Uri(evtGridOptions.ConnectionString), new AzureKeyCredential(evtGridOptions.AccessKey));
        }

        protected virtual CloudEvent BuildCloudEvent(string subscriptionName, string eventId, object eventData)
        {
            // The templated path in DefaultEventBusSubscriptionsManager produces a JToken
            // (JsonConvert.DeserializeObject(string) returns JObject/JArray/JValue).
            // CloudEvent serializes `data` with System.Text.Json, which has no converter for
            // Newtonsoft's JToken — primitive JValues serialize as []. Send the rendered JSON
            // verbatim as BinaryData to bypass STJ on the templated path.
            if (eventData is JToken jt)
            {
                var json = jt.ToString(Newtonsoft.Json.Formatting.None);
                return new CloudEvent(
                    subscriptionName,
                    eventId,
                    BinaryData.FromString(json),
                    "application/json",
                    CloudEventDataFormat.Json);
            }

            return new CloudEvent(subscriptionName, eventId, eventData);
        }
    }
}
