using AutomationHUB.DeviceContainer;
using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Shared.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IDeviceConnectorFactory, DeviceConnectorFactory>();
builder.Services.AddSingleton<IByteDataProcessorFactory, ByteDataProcessorFactory>();
builder.Services.AddSingleton<JsonDeviceConfigLoader>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Debug);
});

var host = builder.Build();
host.Run();
