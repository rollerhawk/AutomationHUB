using AutomationHUB.Messaging.Attributes;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Nats.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AutomationHUB.Messaging.Nats
{
    /// <summary>
    /// Builds a NATS Topic for a message
    /// </summary>
    public class NatsMessageTopicBuilder : INatsMessageTopicBuilder
    {
        private readonly string _prefix;
        private readonly INatsDomainTopicBuilder _domainTopicBuilder;

        public NatsMessageTopicBuilder(IServiceProvider sp, INatsDomainTopicBuilder domainTopicBuilder)
        {
            _prefix = sp.GetRequiredService<IOptions<NatsOptions>>().Value.Prefix;
            _domainTopicBuilder = domainTopicBuilder;

            var registeredTypes = sp.GetServices<INATSPublishable>().Select(x => x.GetType()).ToList();

            foreach (var type in registeredTypes)
            {
                CheckAttributes(type);
            }
        }

        private static void CheckAttributes(Type type)
        {
            if (!type.GetProperties().Any(x => x.GetCustomAttribute<NATSEntityAttribute>(true) == null))
            {
                throw new ArgumentException($"Type {type.Name} does not have a NATSEntityAttribute");
            }
            if (!type.GetProperties().Any(x => x.GetCustomAttribute<NATSEntityIdAttribute>(true) == null))
            {
                throw new ArgumentException($"Type {type.Name} does not have a NATSEntityIdAttribute");
            }
        }

        public string GetMessageTopic(INATSPublishable message)
        {
            return GetTopicFromMessage(message).ToLowerInvariant();            
        }

        private string GetTopicFromMessage(INATSPublishable message)
        {
            Type messageType = message.GetType();
            
            var domain = _domainTopicBuilder.GetDomainTopic(messageType);

            var props = messageType.GetProperties();

            var entity = props.FirstOrDefault(x => x.GetCustomAttribute<NATSEntityAttribute>(true) != null) ?? throw new ArgumentException($"Type {messageType.Name} does not have a NATSEntityAttribute");
            var entityId = props.FirstOrDefault(x => x.GetCustomAttribute<NATSEntityIdAttribute>(true) != null) ?? throw new ArgumentException($"Type {messageType.Name} does not have a NATSEntityIdAttribute");
            return $"{domain}.{entity.GetValue(message)}.{entityId.GetValue(message)}";
        }
    }
}
