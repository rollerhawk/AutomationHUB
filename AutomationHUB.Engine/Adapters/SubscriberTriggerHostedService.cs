using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Engine.Adapters;
public class SubscriberTriggerHostedService : BackgroundService
{
    private readonly ISubscriber _subscriber;
    private readonly IMessageConsumerResolver _resolver;
    private readonly ILogger<SubscriberTriggerHostedService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly string _subjectPattern;

    public SubscriberTriggerHostedService(
        ISubscriber subscriber,
        IMessageConsumerResolver resolver,
        IConfiguration config,
        ILogger<SubscriberTriggerHostedService> logger)
    {
        _subscriber = subscriber;
        _resolver = resolver;
        _logger = logger;
        _subjectPattern = config["Subscriber:Subject"] ?? "devices.*.*";
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

                // löse den passenden Consumer
                await _resolver.ConsumeAsync(message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        });

        return Task.CompletedTask;
    }
}

