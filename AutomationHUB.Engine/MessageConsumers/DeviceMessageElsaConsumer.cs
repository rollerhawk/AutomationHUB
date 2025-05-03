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
        IWorkflowRuntime runtime,
        ILogger<DeviceMessageElsaConsumer> logger) : IMessageConsumer<DeviceMessage>
    {
        private readonly IWorkflowRuntime _runtime = runtime;
        private readonly ILogger<DeviceMessageElsaConsumer> _logger = logger;

        public async Task HandleAsync(DeviceMessage message, CancellationToken ct)
        {
            _logger.LogTrace("Handling DeviceMessage for {DeviceId}", message.DeviceId);

            var client = await _runtime.CreateClientAsync(ct);

            _logger.LogTrace("Creating Elsa client for DeviceMessage");

            // 2) Erstelle das Handle für genau deine Event-Activity
            //    Der TypeName muss exakt mit dem übereinstimmen, was Elsa intern registriert hat.
            var activityTypeName = ActivityTypeNameHelper.GenerateTypeName<DeviceMessageEvent>();

            _logger.LogTrace("Generated ActivityTypeName: {ActivityTypeName}", activityTypeName);

            var handle = new ActivityHandle
            {
                // Name der Activity
                ActivityId = activityTypeName,
                //zB. Scanner1, Waage3 usw.
                ActivityInstanceId = message.AutomationID
            };

            // 3) Pack dein DTO als Input in den Request
            var request = new RunWorkflowInstanceRequest
            {
                ActivityHandle = handle,
                Input = new Dictionary<string, object>
                {
                    { nameof(DeviceMessage), message }
                },
            };

            _logger.LogTrace("Running instance with request: {Request}", request);

            await client.RunInstanceAsync(request, ct);

            _logger.LogTrace("DeviceMessage processed");
        }
    }
}
