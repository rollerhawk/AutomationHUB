using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace AutomationHUB.Messaging.Nats;

public class NatsSubscriber(IConnection connection, ILogger<NatsSubscriber> logger) : ISubscriber, IDisposable
{
    private readonly IConnection _connection = connection;
    private readonly ILogger<NatsSubscriber> _logger = logger;

    public void Subscribe(string subject, Func<byte[], CancellationToken, Task> handler)
    {
        var subscription = _connection.SubscribeAsync(subject);
        subscription.MessageHandler += async (_, args) =>
        {
            _logger.LogDebug("Received message on subject {Subject}", args.Message.Subject);
            await handler(args.Message.Data, CancellationToken.None);
            _logger.LogDebug("Processed message on subject {Subject}", args.Message.Subject);
        };
        subscription.Start();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
