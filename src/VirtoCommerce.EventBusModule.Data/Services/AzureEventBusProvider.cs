using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Services;

namespace VirtoCommerce.EventBusModule.Data.Services
{
    public class AzureEventBusProvider : IEventBusProvider
    {
        public virtual async Task<SendEventResult> SendEventAsync()
        {
            var result = new SendEventResult();

            var client = CreateClient();

            if (client != null)
            {
                //todo: create from event
                var events = new List<CloudEvent>();
                //var cloudEvent = new CloudEvent("", "", null);

                try
                {
                    var eventGridResponse = await client.SendEventsAsync(events);

                    result.StatusCode = eventGridResponse.Status;
                    result.ResponseResult = eventGridResponse.ReasonPhrase;
                }
                catch (RequestFailedException requestFailedEx)
                {
                    result.StatusCode = requestFailedEx.Status;
                    result.ResponseResult = requestFailedEx.ErrorCode;
                    result.ErrorMessage = requestFailedEx.Message;
                }
            }
            else
            {
                result.ErrorMessage = "Either key or endpoint are empty";
            }

            return result;
        }

        private EventGridPublisherClient CreateClient()
        {
            //todo: get from event
            var topicEndpoint = "";
            var topicAccessKey = "";

            EventGridPublisherClient result = null;

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
