using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutomationMessageConsumer<TMessage, TImplementation>(this IServiceCollection services) where TImplementation : class, IMessageConsumer<TMessage> where TMessage : AutomationMessage
        {
            services.AddScoped<IMessageConsumer<TMessage>, TImplementation>();
            return services;
        }
    }
}
