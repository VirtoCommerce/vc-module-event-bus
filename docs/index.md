# Overview

[![CI status](https://github.com/VirtoCommerce/vc-module-catalog/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-catalog/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-catalog&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-catalog) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-catalog&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-catalog) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-catalog&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-catalog) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-catalog&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-catalog)

The module enables you to be notified of new messages or changes via a Message Queue of your choice.

The module is used to trigger an asynchronous background process in response to an event on the Virto Commerce platform.

As a payload, an Event delivers one of the predefined Messages or any Change to a resource.

That enables event-driven, reactive programming. It uses a publish-subscribe model. Publishers emit events but have no expectation about which events are handled. Subscribers decide which events they want to handle.

The event description is based on CloudEvents: "specification for describing event data in a common way".

## Key features
* Notify of new messages or changes from any module
* Destination to:
    * [Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid)
    * Contact us if you need a new Destination
* [CloudEvents](https://cloudevents.io/) format
* Support multiple destination providers
* Configurable via API
* High Performance

## Example uses for Event Bus

### Serverless application architectures
Event Bus connects Virto Commerce and event handlers. For example, use Azure Event Grid to instantly trigger a serverless function to run currency exchange each time a new price is added to a price list.


### Approval process automation
Event Bus allows you to speed automation and simplify approval process enforcement. For example, Azure Event Grid can notify Azure Automation when a new order is created, or a new customer registered. These events can be used to automatically check that entity configurations are compliant, put metadata, change status, or send an email notification.


### Application integration
Event Bus connects your app with other services. For example, create an application topic to send Virto Commerce event data to Event Grid and take advantage of its reliable delivery, advanced routing, and direct integration with Azure.
Alternatively, you can use Event Grid with Logic Apps to process data anywhere, without writing any code.


## Destinations

### Azure Event Grid

[Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid/) can be used to push messages to Azure Functions, HTTP endpoints (webhooks), and several other Azure tools.

Azure Event Grid supports CloudEvents 1.0. And Azure Event Grid client library also supports sending/receiving events in the form of CloudEvents.

To connect Azure Event Grid, you will need to set:

* `provider` - String - Ex: "AzureEventGrid"
* `connectionString` - String - The URI of the Topic
* `accessKey` - String - Partially hidden on retrieval

To set up a subscription with Azure Event Grid you first need to create a Topic in the [Azure Portal](https://azure.microsoft.com/en-us/services/event-grid/). When creating your Event Grid topic, you need to set the input schema to “CloudEvents v1.0” in the “Advanced” tab. To allow Virto Commerce platform to push messages to your Topic, you need to provide an access key. These can also be found in the Azure Portal after creating the Topic in the section Access Keys.

## Scenarios

!!! note
    All operations are accessible via Rest API only. You will need to create [an API Key and grant permission before the call](https://virtocommerce.com/docs/latest/user-guide/security/#generate-api-key).

### List of actual event/resources
As an API Client, I want to Get a list of actual events/resources, so that I know the existing events and can create a subscription.

Endpoint: `/api/eventbus/events`

Method: `GET`

Request: `/api/eventbus/events?skip=0&take=20`

Response:
```json
[
  {
    "id": "VirtoCommerce.CatalogModule.Core.Events.ProductChangedEvent"
  },
  ...
]
```


### Add event subscription
As an API Client, I want to Add an Event Subscription, so that I can receive a set of the events in a particular Event Provider.

Endpoint: `/api/eventbus/subscriptions`

Method: `PUT`

Request Body:
```json
{
  "provider": "AzureEventGrid",
  "connectionString": "https://demo-demo.eastus-1.eventgrid.azure.net/api/events",
  "accessKey": "TXSO992J8UQ420HucBgsCs8qiDjFPerCYp0mS6hceM4=",
  "eventIds": [
    "VirtoCommerce.CatalogModule.Core.Events.ProductChangedEvent"
  ]
}
```

### Update event subscription
As an API Client, I want to Update the Event Subscription, so that I can actualize the set of events.

Endpoint: `/api/eventbus/subscriptions`

Method: `POST`

Request: `/api/eventbus/subscriptions`

Request Body:
```json
{
  "provider": "AzureEventGrid",
  "connectionString": "https://demo-demo.eastus-1.eventgrid.azure.net/api/events",
  "accessKey": "TXSO992J8UQ420HucBgsCs8qiDjFPerCYp0mS6hceM4=",
  "eventIds": [
    "VirtoCommerce.CatalogModule.Core.Events.ProductChangedEvent",
    "VirtoCommerce.CatalogModule.Core.Events.CategoryChangedEvent",
  ]
}
```


### View list of subscriptions
As an API Client, I want to See the Event Subscriptions, so that I know which Subscriptions exist.

Endpoint: `/api/eventbus/subscriptions/search`

Method: `POST`

Request: `/api/eventbus/subscriptions/search`

Request Body:
```json
{
  "skip": 0,
  "take": 20
}
```

Response:
```json
{
  "totalCount": 3,
  "results": [
    {
      "provider": "AzureEventGrid",
      "connectionString": "https://demo-demo.eastus-1.eventgrid.azure.net/api/events",
      "accessKey": "TXSO992J8UQ420HucBgsCs8qiDjFPerCYp0mS6hceM4=",
      "status": 200,
      "events": [
        {
          "subscriptionId": "0819d3a7-9040-4f7f-b444-12b3cd508e46",
          "eventId": "VirtoCommerce.CatalogModule.Core.Events.ProductChangedEvent",
          "createdDate": "2021-01-29T09:56:10.7840966Z",
          "modifiedDate": "2021-01-29T09:56:10.7840966Z",
          "createdBy": "admin",
          "modifiedBy": "admin",
          "id": "fb905b9b-2366-408b-a92c-dde2a7d3d02f"
        }
      ],
      "createdDate": "2021-01-29T09:56:10.7273016Z",
      "modifiedDate": "2021-01-29T10:07:41.8614757Z",
      "createdBy": "admin",
      "modifiedBy": "admin",
      "id": "0819d3a7-9040-4f7f-b444-12b3cd508e46"
    },
    ...
    ]
}

```


### Remove event subscription
As an API Client, I want to Remove an Event Subscription, so that I can stop receiving events.

Endpoint: `/api/eventbus/subscriptions/{id}`

Method: `DELETE`

Request: `/api/eventbus/subscriptions/{id}`


### View health status
A subscription has `status` and `errorMessage` properties which can help to identify the problem.

!!! note
    This property keeps the status of the last operation. Use Azure Application Insights to monitor and review historical data.


Success:
```json
    "status": 200
```

Error:
```json
    "status": 401,
    "errorMessage": "Service request failed.\r\nStatus: 401 (The request authorization key is not authorized for QA-VC-AZUREEVENTGRID.EASTUS-1.EVENTGRID.AZURE.NET. This is due to the reason: The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. . Report '085078ab-df03-49f7-bdde-c1cead053943:9:1/29/2021 11:56:00 AM (UTC)' to our forums for assistance or raise a support ticket.)\r\n\r\nContent:\r\n{\r\n    \"error\": {\r\n        \"code\": \"Unauthorized\",\r\n        \"message\": \"The request authorization key is not authorized for QA-VC-AZUREEVENTGRID.EASTUS-1.EVENTGRID.AZURE.NET. This is due to the reason: The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters. . Report '085078ab-df03-49f7-bdde-c1cead053943:9:1/29/2021 11:56:00 AM (UTC)' to our forums for assistance or raise a support ticket.\",\r\n        \"details\": [{\r\n          ",
```


## Data Model

### Event Subscription
```cs
    public class SubscriptionInfo : AuditableEntity
    {
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string AccessKey { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public SubscriptionEvent[] Events { get; set; }
    }

    public class SubscriptionEvent : AuditableEntity
    {
        public string SubscriptionId { get; set; }
        public string EventId { get; set; }
    } 
```

* `Provider` - String - Required - Provider Name. Ex: AzureEventGrid
* `ConnectionString` - String - Required - Connection string for specific provider. Ex: The URI of the Topic.
* `Status` - Int - Optional - Subscription health status. Http response status code from the last operation for specific provider. Ex: 200.
* `ConnectionString` - String - Optional - Error message from the last operation.
* `SubscriptionEvent` - String - Required - Array of the events.



### Event

```cs
    public class EventData
    {
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string EventId { get; set; }
    }
```

* `ObjectId` - String - Object Unique Key
* `ObjectType` - String - Object Type
* `EventId` - String - Required - Event Id
