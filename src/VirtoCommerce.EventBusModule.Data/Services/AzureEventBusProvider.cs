using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class AzureEventBusProvider : EventBusProvider
    {
        public override async Task<SendEventResult> SendEventAsync(SubscriptionInfo subscription, IList<EventData> events)
        {
            var result = new SendEventResult();

            try
            {
                var client = new EventGridPublisherClient(new Uri(subscription.ConnectionString), new AzureKeyCredential(subscription.AccessKey));

                var cloudEvents = events.Select(x => new CloudEvent(subscription.Id ?? nameof(AzureEventBusProvider), x.EventId, x)).ToList();

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
    }
}
