using AutomationHUB.Messaging.Attributes;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Nats.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AutomationHUB.Messaging.Nats
{
    public class NatsDomainTopicBuilder : INatsDomainTopicBuilder
    {
        private readonly string _prefix;

        public NatsDomainTopicBuilder(IServiceProvider sp)
        {
            _prefix = sp.GetRequiredService<IOptions<NatsOptions>>().Value.Prefix;

            var registeredTypes = sp.GetServices<INATSPublishable>().Select(x => x.GetType()).ToList();

            foreach (var type in registeredTypes)
            {
                CheckAttributes(type);
            }
        }

        private static void CheckAttributes(Type type)
        {
            if (type.GetCustomAttribute<NATSDomainAttribute>(true) == null)
            {
                throw new ArgumentException($"Type {type.Name} does not have a NATSDomainAttribute");
            }
        }

        public string GetDomainTopic(Type messageType)
        {
            var type = messageType.GetCustomAttributes(typeof(NATSDomainAttribute), true).FirstOrDefault() as NATSDomainAttribute ?? throw new ArgumentException($"Type {messageType.Name} does not have a NATSDomainAttribute"); ;
            string topic = type.Domain;
            if (!string.IsNullOrEmpty(_prefix))
            {
                topic = $"{_prefix}.{topic}";
            }
            return topic.ToLowerInvariant();
        }
    }
}
