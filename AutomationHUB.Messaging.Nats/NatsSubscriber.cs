using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace AutomationHUB.Messaging.Nats;

/// <summary>
/// Nats Subscriber
/// Einstellbarer Topic
/// Rückgabe in Bytes
/// </summary>
/// <param name="connection"></param>
/// <param name="logger"></param>
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
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Generischer Nats Subscriber
/// Automatische ermittlung des Topics anhand des INATSPublishable
/// </summary>
/// <typeparam name="T">Rückgabewert ist der Typ des INATSPublishable</typeparam>
/// <param name="topicBuilder"></param>
/// <param name="connection"></param>
/// <param name="logger"></param>
public class NatsSubscriber<T>(INatsMessageTopicBuilder topicBuilder, IConnection connection, ILogger<NatsSubscriber<T>> logger) : NatsSubscriber(connection, logger), ISubscriber<T> where T : INATSPublishable
{
    private readonly INatsMessageTopicBuilder _topicBuilder = topicBuilder;
    public string GetSubject(T message)
    {
        return _topicBuilder.GetMessageTopic(message);
    }
    public void Subscribe(Func<T, CancellationToken, Task> handler)
    {
        var subject = GetSubject(default!);
        Subscribe(subject, async (data, ct) =>
        {
            var message = JsonSerializer.Deserialize<T>(data);
            if (message != null)
            {
                await handler(message, ct);
            }
        });
    }
}
