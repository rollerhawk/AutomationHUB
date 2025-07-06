using AutomationHUB.DeviceContainer.Bootstrapper;
using AutomationHUB.DeviceContainer.Extensions;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Nats.Extensions;
using AutomationHUB.Shared.Configuration;
using Microsoft.Extensions.Hosting;

var host = DeviceContainerBootstrapper.BuildHostForApp(args);
host.Run();