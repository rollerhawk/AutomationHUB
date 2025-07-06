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
        public static IServiceCollection ConfigureNatsOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NatsOptions>(configuration.GetSection("Nats"));
            return services;
        }

        public static IServiceCollection ConfigureNatsOptions(this IServiceCollection services, NatsOptions options)
        {
            services.AddSingleton<IOptions<NatsOptions>>(Microsoft.Extensions.Options.Options.Create(options));
            return services;
        }

        public static IServiceCollection AddNats(this IServiceCollection services)
        {
            services.AddSingleton<IConnection>(sp =>
            {
                var cfOpts = ConnectionFactory.GetDefaultOptions();
                cfOpts.Url = sp.GetRequiredService<IOptions<NatsOptions>>().Value.Url;
                return new ConnectionFactory().CreateConnection(cfOpts);
            });

            services.AddSingleton<INatsDomainTopicBuilder, NatsDomainTopicBuilder>();
            services.AddSingleton<INatsMessageTopicBuilder, NatsMessageTopicBuilder>();
            services.AddSingleton<IPublisher, NatsPublisher>();
            return services;
        }

        /// <summary>
        /// Non-Generic Byte Data NatsSubscriber
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNatsSubscriber(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriber, NatsSubscriber>();
            return services;
        }
    }
}
