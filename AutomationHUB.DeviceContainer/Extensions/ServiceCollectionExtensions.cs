using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Nats;
using AutomationHUB.Messaging.Nats.Configuration;
using AutomationHUB.Shared.Configuration;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.DeviceContainer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDeviceContainerDependencies(this IServiceCollection services)
        {
            services.AddHostedService<DeviceService>();
            services.AddSingleton<IDeviceConnectorFactory, DeviceConnectorFactory>();
            services.AddSingleton<IByteDataProcessorFactory, ByteDataProcessorFactory>();
            services.AddSingleton<JsonDeviceConfigLoader>();

            services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Debug);
            });
            return services;
        }
    }
}
