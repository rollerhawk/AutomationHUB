using AutomationHUB.Messaging.Interfaces;
using NATS.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Nats
{
    public class NatsPublisher(IConnection connection, INatsMessageTopicBuilder topicBuilder) : IPublisher
    {
        private readonly IConnection _connection = connection;
        private readonly INatsMessageTopicBuilder _topicBuilder = topicBuilder;

        public string Publish(INATSPublishable message)
        {
            var subject = _topicBuilder.GetMessageTopic(message);
            var payload = message.ToNATSPayload();
            _connection.Publish(subject, payload);
            return subject;
        }
    }
}
