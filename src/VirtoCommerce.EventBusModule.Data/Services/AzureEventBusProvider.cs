using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
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

            try
            {
                var cloudEvents = new List<CloudEvent>();

                foreach (var @event in events)
                {
                    // Currently, AzureEventBusProvider does not have azure-specific event translation settings.
                    // Here we can add those in the future if needed.
                    // To allow this you should make a descendant from ProviderSpecificEventSettings,
                    // then add deserialization from @event.Subscription.EventSettings

                    var subscriptionName = @event.Subscription.Name ?? nameof(AzureEventBusProvider);
                    var eventId = @event.Payload.EventId;
                    var eventData = Array.Empty<object>();

                    if (string.IsNullOrEmpty(@event.Subscription.PayloadTransformationTemplate) && @event.Payload.Arg is IEvent nativeEvent)
                    {
                        // Implement default behavior equal to previous version
                        // of eventbus module (compatibility)

                        var entities = nativeEvent.GetObjectsWithDerived<IEntity>()
                             .Select(x => new { ObjectId = x.Id, ObjectType = x.GetType().FullName, EventId = eventId });

                        var valueObjects = nativeEvent.GetObjectsWithDerived<ValueObject>()
                            .Select(x => new { ObjectId = x.GetCacheKey(), ObjectType = x.GetType().FullName, EventId = eventId });

                        eventData = entities.Union(valueObjects).ToArray<object>();
                    }

                    if (eventData.Length > 0)
                    {
                        cloudEvents.AddRange(eventData.Select(x => new CloudEvent(subscriptionName, eventId, x)));
                    }
                    else
                    {
                        cloudEvents.Add(new CloudEvent(subscriptionName, eventId, @event.Payload.Arg));
                    }
                }

                var eventGridResponse = await _client.SendEventsAsync(cloudEvents);

                result.Status = eventGridResponse.Status;
            }
            catch (ArgumentException)
            {
                result.Status = StatusCodes.Status400BadRequest;
                result.ErrorMessage = "Either key or endpoint are empty";
            }
            catch (UriFormatException)
            {
                result.Status = StatusCodes.Status400BadRequest;
                result.ErrorMessage = "Invalid endpoint URI format";
            }
            catch (RequestFailedException requestFailedEx)
            {
                result.Status = requestFailedEx.Status;
                result.ErrorMessage = requestFailedEx.Message;
            }
            catch (Exception ex)
            {
                result.Status = StatusCodes.Status500InternalServerError;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public override void SetConnectionOptions(JObject options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var evtGridOptions = options.ToObject<AzureEventGridOptions>();

            _client = new EventGridPublisherClient(new Uri(evtGridOptions.ConnectionString), new AzureKeyCredential(evtGridOptions.AccessKey));
        }
    }
}
