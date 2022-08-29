# Overview

[![CI status](https://github.com/VirtoCommerce/vc-module-event-bus/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-event-bus/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus)

The module enables you to be notified of new Virto Commerce events or changes via a Message Queue of your choice.

The module is used to trigger an asynchronous background process in response to an event on the Virto Commerce platform.

![Event Bus Schema Overview](media/event-bus-overview.PNG)

As a payload, a Virto Commerce Event delivers one of the predefined Messages or any Change to a resource.

That enables event-driven, reactive programming. It uses a publish-subscribe model. Publishers emit events but have no expectation about which events are handled. Subscribers decide which events they want to handle.

## Key features
* Notify of new events from any module;
* Support multiple destination providers;
* Support for custom destination providers (Contact us if you need a new destination);
* Configurable via API as well as thru application configuration (appsettings.json, environment variables, etc.);
* Additional events filtering with JsonPath expression;
* Preprocessing an event data with Liquid-template allows to finely tune the payload for destination provider;
* High Performance;
* Contains one predefined destination provider:
    * [Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid) with [CloudEvents](https://cloudevents.io/)-based data format.

## Example uses for Event Bus

### Serverless application architectures
Event Bus connects Virto Commerce and event handlers. For example, use Azure Event Grid to instantly trigger a serverless function to run currency exchange each time a new price is added to a price list.

### Approval process automation
Event Bus allows you to speed automation and simplify approval process enforcement. For example, Azure Event Grid can notify Azure Automation when a new order is created, or a new customer registered. These events can be used to automatically check that entity configurations are compliant, put metadata, change status, or send an email notification.

### Application integration
Event Bus connects your app with other services. For example, create an application topic to send Virto Commerce event data to Event Grid and take advantage of its reliable delivery, advanced routing, and direct integration with Azure.
Alternatively, you can use Event Grid with Logic Apps to process data anywhere, without writing any code.

## Configuring events translation
There are two ways to configure events translation in the module:
* Configuration options;
* API endpoints.

Both of them shares equal options data structures.

!!! note
    In order to access API endpoints you will need to create [an API Key and grant permission before the call](https://virtocommerce.com/docs/latest/user-guide/security/#generate-api-key).

### Configure providers connections
Each provider connection definition is a link between provider type and connection options. You can have multiple connections to different destinations with use of one provider.

**Provider connection options data structure**

Example:
```json
{
    "Name": "AzureEventGrid cloud",
    "ProviderName": "AzureEventGrid",
    "ConnectionOptionsSerialized": "{\"ConnectionString\": \"https://*.*.eventgrid.azure.net/api/events\", \"AccessKey\": \"kvpXffggvvMiNjBeBKdroX1r45UvZloXMwlM7i1TyqoiI=\"}"
}
```
#### Description:
|Name|Description|
|-|-|
|Name|Human-readable connection name to distinguish the connection. Should be unique in the configuration|
|ProviderName|Predefined destination provider name. Unique for each type of providers. Refer to the name of desired provider.|
|ConnectionOptionsSerialized|Provider-specific connection options|

### Manage provider connections thru configuration
Add connections array under the key "EventBus:Connections".
Connections configured such manner can't be removed or updated thru REST API.
If you have connections with the same name in DB and configuration, configuration will be preferred.

### Manage provider connections thru REST API

#### **Add new connection**
Endpoint: `/api/eventbus/connections`

Method: `POST`

Request: 
```json
{
  "name": "string",
  "providerName": "string",
  "connectionOptionsSerialized": "string"
}
```
Requst body the same as above in connection option description.

#### **Remove connection by name**
Endpoint: `/api/eventbus/connections/{name}`

Method: `DELETE`

Request parameter: Provider connection name

#### **Update connection**
Endpoint: `/api/eventbus/connections`

Method: `PUT`

Request: 
```json
{
  "name": "string",
  "providerName": "string",
  "connectionOptionsSerialized": "string"
}
```
Requst body the same as above in connection option description.

#### **Get connection by name**
Endpoint: `/api/eventbus/connections/{name}`

Method: `GET`

Request parameter: Provider connection name

Response:
```json
{
  "name": "string", // Connection name
  "providerName": "string", // Provider name
  "connectionOptionsSerialized": "string", // Provider-specific connection options
  "createdDate": "2022-08-26T13:52:55.932Z",
  "modifiedDate": "2022-08-26T13:52:55.932Z",
  "createdBy": "string", // If null, the connection specified in the configuration
  "modifiedBy": "string", // If null, the connection specified in the configuration
  "id": "string" // If null, the connection specified in the configuration
}
```

#### **Search for connections**

Endpoint: `/api/eventbus/connections/{name}`

Method: `GET`

Request:
```json
{
  "name": "string", // Connection name (optional, pass to search by name)
  "providerName": "string", // Provider name (optional, pass to search by provider name)
  "skip": 0, // providers to skip in paged loading
  "take": 0  // providers to take in paged loading
}
```

Response:
```json
{
  "totalCount": 0, 
  "results": [
    {
      "name": "string", // Connection name
      "providerName": "string", // Provider name
      "connectionOptionsSerialized": "string", // Provider-specific connection options
      "createdDate": "2022-08-26T13:52:55.932Z",
      "modifiedDate": "2022-08-26T13:52:55.932Z",
      "createdBy": "string", // If null, the connection specified in the configuration
      "modifiedBy": "string", // If null, the connection specified in the configuration
      "id": "string" // If null, the connection specified in the configuration
    }
  ]
}
```

### Configure subscriptions

Subscription is a rule, that specifies which events should be catched and forwarded to a selected provider connection. Also you can translate event body to fit payload needs of a provider.

**Subscription options data structure**

Example:
```json
{
  "ConnectionName": "AzureEventGrid",
  "Name": "Eventgrid forwarder",
  "JsonPathFilter": "$",
  "PayloadTransformationTemplate": "",
  "EventSettingsSerialized": null,
  "Events": [
      {
          "EventId": "VirtoCommerce.YourModule.Web.Events.NewCompanyRegistrationRequestEvent"
      }
  ]
}
```
#### Description:
|Name|Description|
|-|-|
|ConnectionName|Name of the connection where event data to be forwarded to.|
|Name|Human-readable name to distinguish subscriptions. Should be unique in the configuration.|
|JsonPathFilter|JsonPath-filter expression allows additionally filter events that have specific value in the body. If body applied JsonPath-filter does not give some value, the module doesn't call the provider. There "$" set as a default value that means any event body is OK and translates to the provider.|
|PayloadTransformationTemplate|Optional. You can pass here a liquid-template to transform event data to a different form. If omitted, or null, or an empty string, - the event data passes unchanged to the provider(full body).|
|EventSettingsSerialized|Optional. You can set subscription-specific metadata for the provider as details of event interpretation. Some rules, instructions for the provider, etc.  For example, if you hypothetically have some workflow provider you can prescribe what such provider need to do as a reaction for event catch: start new workflow chain or signal existing workflow instance... The value is different from provider to provider. Please read provider instruction.|
|Events|Array of the events fullnames that this subscription should catch.|

### Manage subscriptions thru configuration
Add subscriptions array under the key "EventBus:Subscriptions".
Subscriptions configured such manner can't be removed or updated thru REST API.
If you have subscriptions with the same name in DB and configuration, configuration will be preferred.

### Manage subscriptions thru REST API

#### **Get specific subscription by name**
Endpoint: `/api/eventbus/subscriptions/{name}`

Method: `GET`

Request parameter: Subscription name

Response:
```json
{
  "name": "string",
  "connectionName": "string",
  "jsonPathFilter": "string",
  "payloadTransformationTemplate": "string",
  "eventSettingsSerialized": "string",
  "events": [
    {
      "eventId": "string"
    }
  ],
  "createdDate": "2022-08-29T11:30:09.038Z",
  "modifiedDate": "2022-08-29T11:30:09.038Z",
  "createdBy": "string", // If null, the subscription specified in the configuration
  "modifiedBy": "string", // If null, the subscription specified in the configuration
  "id": "string" // If null, the subscription specified in the configuration
}
```
#### **Register new subscription in the database**

Endpoint: `/api/eventbus/subscriptions`

Method: `POST`

Request Body (look above at the subscription option description):
```json
{
  "name": "string",
  "connectionName": "string",
  "jsonPathFilter": "string",
  "payloadTransformationTemplate": "string",
  "eventSettingsSerialized": "string",
  "events": [
    {
      "eventId": "string"
    }
  ]
}
```

#### **Update existing subscription (DB-registered only)**
As an API Client, I want to Update the Event Subscription, so that I can actualize the set of events.

Endpoint: `/api/eventbus/subscriptions`

Method: `PUT`

Request Body (look above at the subscription option description)::
```json
{
  "name": "string",
  "connectionName": "string",
  "jsonPathFilter": "string",
  "payloadTransformationTemplate": "string",
  "eventSettingsSerialized": "string",
  "events": [
    {
      "eventId": "string"
    }
  ]
}
```

#### **Delete existing subscription by name (DB-registered only)**
As an API Client, I want to Remove an Event Subscription, so that I can stop receiving events.

Endpoint: `/api/eventbus/subscriptions/{name}`

Method: `DELETE`


#### **View list of subscriptions or search for existing subscriptions (DB registered + configuration registered)**
As an API Client, I want to See the Event Subscriptions, so that I know which Subscriptions exist.

Endpoint: `/api/eventbus/subscriptions/search`

Method: `POST`

Request Body:
```json
{
  "name": "string", // Subscription name (optional, pass to search by name)
  "connectionName": "string", // Provider connection name (optional, pass to search by it)
  "eventIds": [ // Optional. Pass to search subscriptions by event ids
    "string"
  ],
  "skip": 0, // subscriptions to skip in paged loading
  "take": 0 // subscriptions to take in paged loading
}
```

Response:
```json
{
  "totalCount": 0,
  "results": [
    {
      "name": "string",
      "connectionName": "string",
      "jsonPathFilter": "string",
      "payloadTransformationTemplate": "string",
      "eventSettingsSerialized": "string",
      "events": [
        {
          "eventId": "string",
        }
      ],
      "createdDate": "2022-08-29T11:53:53.653Z",
      "modifiedDate": "2022-08-29T11:53:53.653Z",
      "createdBy": "string", // If null, the subscription specified in the configuration
      "modifiedBy": "string", // If null, the subscription specified in the configuration
      "id": "string" // If null, the subscription specified in the configuration
    }
  ]
}

```
### Discovering actual list of events/resources
How to know the full list of existing events to properly create a subscription? The answer is here.

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

## Destination providers

### Azure Event Grid Provider

#### Overview

[Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid/) can be used to push messages to Azure Functions, HTTP endpoints (webhooks), and several other Azure tools.

Azure Event Grid supports CloudEvents 1.0. And Azure Event Grid client library also supports sending/receiving events in the form of CloudEvents.

The event bus module contains ready to use Azure Event Grid provider.
To connect to it, you need to define provider connection with provider name "AzureEventGrid" and fill connection options data structure (ConnectionOptionsSerialized field) with the value like:

```json
{
  "ConnectionString": "https://*.*.eventgrid.azure.net/api/events", 
  "AccessKey": "kvpXffggvvMiNjBeBKdroX1r45UvZloXMwlM7i1TyqoiI="
}
```

* `connectionString` - String - The URI of the Topic
* `accessKey` - String - Partially hidden on retrieval

To set up a subscription with Azure Event Grid you first need to create a Topic in the [Azure Portal](https://azure.microsoft.com/en-us/services/event-grid/). When creating your Event Grid topic, you need to set the input schema to “CloudEvents v1.0” in the “Advanced” tab. To allow Virto Commerce platform to push messages to your Topic, you need to provide an access key. These can also be found in the Azure Portal after creating the Topic in the section Access Keys.

The EventSettingsSerialized option of the subscription not used by this provider and should be omitted.

#### Error Handling
Event Grid provides durable delivery. It delivers each message at least once for each subscription. Events are sent to the registered endpoint of each subscription immediately. If an endpoint doesn't acknowledge receipt of an event, Event Grid retries delivery of the event.
More details in [Azure Portal](https://docs.microsoft.com/en-us/azure/event-grid/delivery-and-retry)



#### Default event data model for Azure Event Grid
As mentioned above, you can specify the payload tranformation thru Liquid-template with  subscription payloadTransformationTemplate option.
But, what is happened, if you omit this option? Then, Event Grid provider will apply the following structure as a payload in CloudEvents format, example:
```json
{​​​​​
    "ObjectId": "4038511b-604a-4031-9aba-775bbac43a39",
    "ObjectType": "VirtoCommerce.OrdersModule.Core.Model.CustomerOrder",
    "EventId": "VirtoCommerce.OrdersModule.Core.Events.OrderChangedEvent"
}
```
* `ObjectId` - String - Object unique identifier
* `ObjectType` - String - Full name of related object type
* `EventId` - String - Required. Full name of eventId

The Event Grid provider discovers all the objects related to the passed event and generates payloads for each of them.

#### Sample event in CloudEvents 1.0 JSON format
```json
{​​​​​
  "id": "9ec0a767-5789-4149-83ea-bd227570e54a",
  "source": "399c9dda-aff9-4bd9-87b4-326dbe2815a9",
  "data": {​​​​​
    "ObjectId": "4038511b-604a-4031-9aba-775bbac43a39",
    "ObjectType": "VirtoCommerce.OrdersModule.Core.Model.CustomerOrder",
    "EventId": "VirtoCommerce.OrdersModule.Core.Events.OrderChangedEvent"
  }​​​​​,
  "type": "VirtoCommerce.OrdersModule.Core.Events.OrderChangedEvent",
  "time": "2021-02-26T08:45:57.3896153Z",
  "specversion": "1.0",
  "traceparent": "00-22fb7c5208a34c41811cca2715e8d71e-d856ef9e25234f41-00"
}​​​​​
```


## Health status, search for fails log
There is API endpoint allowing to view fails log.


Endpoint: `/api/eventbus/logs/search`

Method: `POST`

Request: 
```json
{
  "providerConnectionName": "string", // Optional. Pass to filter the log by provider connection
  "startCreatedDate": "2022-08-29T12:42:07.944Z", // Start date of event occurrence
  "endCreatedDate": "2022-08-29T12:42:07.944Z", // Start date of event occurrence
  "skip": 0, // errors to skip in paged loading
  "take": 0 // errors to take in paged loading
}
```

Response:
```json
{
  "totalCount": 0,
  "results": [
    {
      "providerName": "string", // Provider connection name
      "status": 0, // Error status
      "errorMessage": "string", // Error message
      "createdDate": "2022-08-29T12:47:07.793Z", // Date of occurrence
    }
  ]
}
```
Records in response are always ordered by descending date of occurrence.

## How to send custom event from Virto Commerce
The module reads list of the events from installed modules.
If you want to send a new event, you need to create a new module and [raise a Virto Commerce Event](https://virtocommerce.com/docs/latest/fundamentals/extensibility/extending-using-events/).
After this the event will be accessible via API and you can create an subscription.  

## Support for custom destination providers 
Contact us if you need a new destination.
