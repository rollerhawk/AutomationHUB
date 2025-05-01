using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Nats;
using AutomationHUB.Messaging.Nats.Configuration;
using AutomationHUB.Shared.Configuration;
using AutomationHUB.Shared.Interfaces;
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
        public static IServiceCollection AddDeviceContainerDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<IDeviceConnectorFactory, DeviceConnectorFactory>();
            services.AddSingleton<IByteDataProcessorFactory, ByteDataProcessorFactory>();
            services.AddSingleton<JsonDeviceConfigLoader>();

            services.Configure<NatsOptions>(config.GetSection("Nats"));

            services.AddSingleton<IConnection>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NatsOptions>>().Value;
                var cfOpts = ConnectionFactory.GetDefaultOptions();
                cfOpts.Url = options.Url;
                return new ConnectionFactory().CreateConnection(cfOpts);
            });

            services.AddSingleton<IPublisher<DeviceMessage>, NatsPublisher<DeviceMessage>>();

            services.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Debug);
            });
            return services;
        }
    }
}
