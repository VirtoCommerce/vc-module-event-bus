using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
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

            var clientCreateResult = CreateClient(subscription);

            if (clientCreateResult.Client != null)
            {
                var cloudEvents = events.Select(x => new CloudEvent(subscription.Id ?? nameof(AzureEventBusProvider), x.EventId, x)).ToList();

                try
                {
                    var eventGridResponse = await clientCreateResult.Client.SendEventsAsync(cloudEvents);

                    result.Status = eventGridResponse.Status;
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
            }
            else
            {
                result.Status = StatusCodes.Status400BadRequest;
                result.ErrorMessage = clientCreateResult.Error;
            }

            return result;
        }

        private PublisherClientCreateResult CreateClient(SubscriptionInfo subscription)
        {
            var topicEndpoint = subscription.ConnectionString;
            var topicAccessKey = subscription.AccessKey;

            var result = new PublisherClientCreateResult();

            if (!string.IsNullOrEmpty(topicEndpoint) && !string.IsNullOrEmpty(topicAccessKey))
            {
                if (Uri.TryCreate(topicEndpoint, UriKind.Absolute, out var topicEndpointUri)
                    && (topicEndpointUri.Scheme == Uri.UriSchemeHttp || topicEndpointUri.Scheme == Uri.UriSchemeHttps))
                {
                    result.Client = new EventGridPublisherClient(topicEndpointUri, new AzureKeyCredential(topicAccessKey));
                }
                else
                {
                    result.Error = "Invalid endpoint URI format";
                }
            }
            else
            {
                result.Error = "Either key or endpoint are empty";
            }

            return result;
        }


        private class PublisherClientCreateResult
        {
            public EventGridPublisherClient Client { get; set; }

            public string Error { get; set; }
        }
    }
}
