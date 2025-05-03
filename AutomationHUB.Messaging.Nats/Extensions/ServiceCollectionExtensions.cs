using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Nats.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Nats.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNats(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<NatsOptions>(config.GetSection("Nats"));

            services.AddSingleton<IConnection>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NatsOptions>>().Value;
                var cfOpts = ConnectionFactory.GetDefaultOptions();
                cfOpts.Url = options.Url;
                return new ConnectionFactory().CreateConnection(cfOpts);
            });
            return services;
        }

        public static IServiceCollection AddNatsPublisher<T>(this IServiceCollection services)
        {
            services.AddSingleton<IPublisher<T>, NatsPublisher<T>>();
            return services;
        }
    }
}
