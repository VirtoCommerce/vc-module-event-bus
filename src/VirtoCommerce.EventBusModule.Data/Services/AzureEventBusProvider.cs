using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class AzureEventBusProvider : IEventBusProvider
    {
        public virtual async Task<SendEventResult> SendEventAsync(SubscriptionInfo subscription, IList<EventData> events)
        {
            var result = new SendEventResult();

            var client = CreateClient(subscription);

            if (client != null)
            {
                var cloudEvents = events.Select(x => new CloudEvent(subscription.Id ?? nameof(AzureEventBusProvider), x.EventId, x)).ToList();

                try
                {
                    var eventGridResponse = await client.SendEventsAsync(cloudEvents);

                    result.Status = eventGridResponse.Status;
                }
                catch (RequestFailedException requestFailedEx)
                {
                    result.Status = requestFailedEx.Status;
                    result.ErrorMessage = requestFailedEx.Message;
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                }
            }
            else
            {
                result.ErrorMessage = "Either key or endpoint are empty";
            }

            return result;
        }

        private EventGridPublisherClient CreateClient(SubscriptionInfo subscription)
        {
            var topicEndpoint = subscription.ConnectionString;
            var topicAccessKey = subscription.AccessKey;

            var result = default(EventGridPublisherClient);

            if (!string.IsNullOrEmpty(topicEndpoint) && !string.IsNullOrEmpty(topicAccessKey))
            {
                result = new EventGridPublisherClient(
                    new Uri(topicEndpoint),
                    new AzureKeyCredential(topicAccessKey));
            }

            return result;
        }
    }
}
