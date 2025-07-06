using AutomationHUB.DeviceContainer.Extensions;
using AutomationHUB.DeviceContainer;
using AutomationHUB.Messaging.Nats.Configuration;
using AutomationHUB.Shared.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Nats.Extensions;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.DeviceContainer.Bootstrapper
{
    public class DeviceContainerBootstrapper
    {
        /// <summary>
        /// Baut einen Host für verwendung im Runtime und startet ihn asynchron.
        /// </summary>
        /// <param name="deviceConfig"></param>
        /// <param name="natsOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IHost BuildAndStartHost(
            DeviceConfiguration deviceConfig,
            NatsOptions natsOptions,
            CancellationToken cancellationToken = default)
        {
            var builder = Host.CreateApplicationBuilder();

            // Manuell konfigurierte Options (statt IConfiguration)
            builder.Services
            .AddDeviceConfigLoader(new InstanceDeviceConfigLoader(deviceConfig))
            .AddDeviceContainerDependencies()
            .AddConsoleLogging()
            .AddSingleton(Options.Create(natsOptions))
            .AddNats();

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DeviceContainerBootstrapper));

            _ = host.RunAsync(cancellationToken).ContinueWith((task) => logger.LogError(task.Exception, "Host Task Failed"), cancellationToken, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

            return host;
        }

        /// <summary>
        /// Baut einen Host für die App und konfiguriert die Abhängigkeiten.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHost BuildHostForApp(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddDeviceContainerDependencies()
                .AddDeviceConfigLoader<JsonDeviceConfigLoader>()
                .AddConsoleLogging()
                .ConfigureNatsOptions(builder.Configuration)
                .AddNats();

            var host = builder.Build();
            return host;
        }
    }
}
