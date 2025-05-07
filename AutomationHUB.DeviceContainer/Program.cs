using AutomationHUB.DeviceContainer.Extensions;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Nats.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDeviceContainerDependencies()
                .AddNats(builder.Configuration);
                

var host = builder.Build();
host.Run();
