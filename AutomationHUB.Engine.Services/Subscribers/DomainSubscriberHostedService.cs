using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutomationHUB.Messaging;
using AutomationHUB.Messaging.Nats;

namespace AutomationHUB.Engine.Services.Subscribers;

public abstract class DomainSubscriberHostedService : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly ILogger<DomainSubscriberHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly string _subjectPattern;

    public DomainSubscriberHostedService(
        string subjectPattern,
        ISubscriber subscriber,        
        ILogger<DomainSubscriberHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _subjectPattern = subjectPattern;
        _subscriber = subscriber;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Abonniere alle Sub-Topics des Domain-Topics
        string natsSubjectString = _subjectPattern + ".>";
        _subscriber.Subscribe(natsSubjectString, HandleEventAsync);
        return Task.CompletedTask;
    }

    private async Task HandleEventAsync(byte[] data, CancellationToken ct)
    {
        try
        {            
            // Deserialisiere polymorph auf AutomationMessage
            var message = JsonSerializer.Deserialize<AutomationMessage>(data, _serializerOptions)!;
            await ProcessMessageAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
    }

    protected virtual async Task ProcessMessageAsync<T>(T message, CancellationToken ct) where T : AutomationMessage
    {
        // ermittle den tatsächlichen CLR-Typ
        var messageType = message.GetType();

        _logger.LogInformation(
                      "Received {MessageType} (@{Subject})",
                      messageType.Name, _subjectPattern);

        // baue das generic Interface IConsumer<messageType>
        var consumerInterface = typeof(IMessageConsumer<>).MakeGenericType(messageType);

        // 1) Erstelle einen neuen Scope pro Nachricht
        using var scope = _serviceProvider.CreateScope();

        // 2) Hole den scoped Consumer aus genau diesem Scope
        var consumer = scope.ServiceProvider
                             .GetRequiredService(consumerInterface);

        var method = consumerInterface.GetMethod(nameof(IMessageConsumer<T>.HandleAsync), new[] { messageType, typeof(CancellationToken) }) ?? throw new InvalidOperationException("HandleAsync(DeviceMessage, CancellationToken) nicht gefunden.");

        // 3) Rufe es auf – Invoke liefert ein object zurück, das ein Task ist
        await (Task)method.Invoke(
            consumer,
            new object[] { message, ct })!;
    }
}

/// <summary>
/// Generische Implementierung für den DomainSubscriberHostedService.
/// Abonniert den gesamten Domain-Topic für den Typ T.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="domainTopicBuilder"></param>
/// <param name="subscriber"></param>
/// <param name="logger"></param>
/// <param name="serviceProvider"></param>
public class DomainSubscriberHostedService<T>(INatsDomainTopicBuilder domainTopicBuilder, ISubscriber subscriber, ILogger<DomainSubscriberHostedService> logger, IServiceProvider serviceProvider) : DomainSubscriberHostedService(domainTopicBuilder.GetDomainTopic(typeof(T)), subscriber, logger, serviceProvider) where T : INATSPublishable
{
}

