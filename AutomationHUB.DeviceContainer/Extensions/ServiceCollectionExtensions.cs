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
        public static IServiceCollection AddConsoleLogging(this IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Debug);
            });
            return services;
        }

        public static IServiceCollection AddDeviceConfigLoader<T>(this IServiceCollection services, T instance = null) where T : class, IDeviceConfigLoader
        {
            if(instance == null)
            {
                services.AddSingleton<IDeviceConfigLoader, T>();
            }
            else
            {
                services.AddSingleton<IDeviceConfigLoader>(instance);
            }
            return services;
        }

        public static IServiceCollection AddDeviceContainerDependencies(this IServiceCollection services)
        {
            services.AddHostedService<DeviceService>();
            services.AddSingleton<IDeviceConnectorFactory, DeviceConnectorFactory>();
            services.AddSingleton<IByteDataProcessorFactory, ByteDataProcessorFactory>();            
            return services;
        }
    }
}
