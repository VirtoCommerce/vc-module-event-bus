# Virto Commerce Event Bus Module

[![CI status](https://github.com/VirtoCommerce/vc-module-event-bus/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-event-bus/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-event-bus&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-event-bus)

This module enables you to be notified of new Virto Commerce events or changes via the message queue of your choice.

The module is used to trigger an asynchronous background process in response to an event on the Virto Commerce platform.

![Event Bus Schema Overview](docs/media/event-bus-overview.PNG)

As a payload, a Virto Commerce event delivers one of the predefined messages or any change to a resource. This enables event-driven reactive programming, which uses a publish-subscribe model. Publishers emit events but have no expectation about which events are handled. Subscribers decide which events they want to handle.

The event description is based on *CloudEvents: "specification for describing event data in a common way*.


## Key Features

* Notification on new events from any module
* Supporting multiple destination providers
* Supporting custom destination providers (contact us if you need a new destination)
* Configurable via API as well as through application configuration (`appsettings.json`, environment variables, etc.)
* Additional event filtering with the `JsonPath` expression
* Preprocessing event data with Liquid template enables fine-tuning the payload for the destination provider
* High performance
* Predefined destination provider:  [Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid) with [CloudEvents](https://cloudevents.io/)-based data format

## Documentation

* [Module Documentation](https://virtocommerce.com/docs/latest/modules/event-bus/)
* [View on GitHub](docs/index.md)


## References

* Deployment: https://virtocommerce.com/docs/latest/developer-guide/deploy-module-from-source-code/
* Installation: https://www.virtocommerce.com/docs/latest/user-guide/modules/
* Home: https://virtocommerce.com
* Community: https://www.virtocommerce.org
* [Download Latest Release](https://github.com/VirtoCommerce/vc-module-event-bus/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
