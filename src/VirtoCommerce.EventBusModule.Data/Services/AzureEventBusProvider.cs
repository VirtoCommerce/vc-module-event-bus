using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class AzureEventBusProvider : EventBusProvider
    {
        private EventGridPublisherClient client;
        public override bool Connect()
        {
            return true;
        }

        public override bool IsConnected()
        {
            return client != null;
        }

        public override async Task<SendEventResult> SendEventsAsync(IEnumerable<Event> events)
        {
            var result = new SendEventResult();

            try
            {
                var cloudEvents = events.Select(@event =>
                {
                    // Currently, AzureEventBusProvider does not have azure-specific event translation settings.
                    // Here we can add those in the future if need
                    // To allow this you should make a descendant from ProviderSpecificEventSettings,
                    // add deserialization from @event.Subscription.EventSettings
                    return new CloudEvent(@event.Subscription.Id ?? nameof(AzureEventBusProvider), @event.Payload.EventId, @event.Payload.Arg);
                }).ToList();

                var eventGridResponse = await client.SendEventsAsync(cloudEvents);

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
            var evtGridOptions = options.ToObject<AzureEventGridOptions>();

            client = new EventGridPublisherClient(new Uri(evtGridOptions.ConnectionString), new AzureKeyCredential(evtGridOptions.AccessKey));
        }
    }
}
