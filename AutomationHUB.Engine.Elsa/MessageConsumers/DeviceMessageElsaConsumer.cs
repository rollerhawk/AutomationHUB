using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Elsa.Workflows.Runtime;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Elsa.MessageConsumers
{
    /// <summary>
    /// Processes DeviceMessage and triggers the corresponding Elsa activity.
    /// </summary>
    public class DeviceMessageElsaConsumer : IMessageConsumer<DeviceMessage>
    {        
        private readonly IEventPublisher _elsaPublisher;
        private readonly ILogger<DeviceMessageElsaConsumer> _logger;

        public DeviceMessageElsaConsumer(
            ILogger<DeviceMessageElsaConsumer> logger, IEventPublisher elsaPublisher)
        {
            _elsaPublisher = elsaPublisher;
            _logger = logger;
        }


        public async Task HandleAsync(DeviceMessage message, CancellationToken ct)
        {
            _logger.LogTrace("Handling DeviceMessage for {DeviceId}", message.Id);

            await _elsaPublisher.PublishAsync(message.Id+"."+message.MessageType, payload: message, cancellationToken: ct);

            _logger.LogTrace("DeviceMessage processed");
        }
    }
}
