using AutomationHUB.Engine.Activities.Events;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.MessageConsumers
{
    /// <summary>
    /// Processes DeviceMessage and triggers the corresponding Elsa activity.
    /// </summary>
    public class DeviceMessageElsaConsumer(
        ILogger<DeviceMessageElsaConsumer> logger, IEventPublisher elsaPublisher, IWorkflowRuntime workflowRuntime) : IMessageConsumer<DeviceMessage>
    {
        private readonly IWorkflowRuntime _workflowRuntime = workflowRuntime;
        private readonly IEventPublisher _elsaPublisher = elsaPublisher;
        private readonly ILogger<DeviceMessageElsaConsumer> _logger = logger;

        public async Task HandleAsync(DeviceMessage message, CancellationToken ct)
        {
            _logger.LogTrace("Handling DeviceMessage for {DeviceId}", message.DeviceId);

            await _elsaPublisher.PublishAsync(message.AutomationID+"."+message.MessageType, payload: message, cancellationToken: ct);

            _logger.LogTrace("DeviceMessage processed");
        }
    }
}
