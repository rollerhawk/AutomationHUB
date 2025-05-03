using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Elsa.Workflows.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Engine.Adapters;
public class SubscriberTriggerHostedService : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly ILogger<SubscriberTriggerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly string _subjectPattern;    

    public SubscriberTriggerHostedService(
        ISubscriber subscriber,
        IConfiguration config,
        ILogger<SubscriberTriggerHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _subscriber = subscriber;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _subjectPattern = config["Subscriber:Subject"] ?? "Scanner.*";
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _subscriber.Subscribe(_subjectPattern, async (data, ct) =>
        {
            try
            {
                // Deserialisiere polymorph auf AutomationMessage
                var message = JsonSerializer.Deserialize<AutomationMessage>(
                    data, _serializerOptions)!;

                _logger.LogInformation(
                  "Received {MessageType} (@{Subject})",
                  message.MessageType, _subjectPattern);

                // ermittle den tatsächlichen CLR-Typ
                var messageType = message.GetType();
                // baue das generic Interface IConsumer<messageType>
                var consumerInterface = typeof(IMessageConsumer<>).MakeGenericType(messageType);

                // 1) Erstelle einen neuen Scope pro Nachricht
                using var scope = _serviceProvider.CreateScope();

                // 2) Hole den scoped Consumer aus genau diesem Scope
                var consumer = scope.ServiceProvider
                                     .GetRequiredService(consumerInterface);

                var method = consumerInterface.GetMethod(nameof(IMessageConsumer<AutomationMessage>.HandleAsync), new[] { messageType, typeof(CancellationToken) }) ?? throw new InvalidOperationException("HandleAsync(DeviceMessage, CancellationToken) nicht gefunden.");

                // 3) Rufe es auf – Invoke liefert ein object zurück, das ein Task ist
                await (Task)method.Invoke(
                    consumer,
                    new object[] { message, ct })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        });       

        return Task.CompletedTask;
    }
}

