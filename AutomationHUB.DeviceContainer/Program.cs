using AutomationHUB.DeviceContainer.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDeviceContainerDependencies(builder.Configuration);

var host = builder.Build();
host.Run();
