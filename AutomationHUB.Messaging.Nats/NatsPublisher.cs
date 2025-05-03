using AutomationHUB.Messaging.Interfaces;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Nats
{
    public class NatsPublisher<T>(IConnection connection) : IPublisher<T>
    {
        private readonly IConnection _connection = connection;

        public void Publish(string subject, byte[] payload)
        {
            _connection.Publish(subject, payload);
        }

        public void Publish(string subject, T message)
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(message);
            Publish(subject, payload);
        }
    }
}
