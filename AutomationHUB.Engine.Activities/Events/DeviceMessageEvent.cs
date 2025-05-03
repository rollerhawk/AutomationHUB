using AutomationHUB.Messaging.Devices;
using Elsa.Workflows;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Engine.Activities.Events;

/// <summary>
/// Elsa Activity for Handling DeviceMessage
/// </summary>
/// <param name="logger"></param>
public class DeviceMessageEvent(ILogger<DeviceMessageEvent> logger) : AutomationMessageEvent<DeviceMessage>(logger)
{
    protected override ValueTask OnEventReceivedAsync(ActivityExecutionContext context, DeviceMessage? input)
    {
        if (input is null)
        {
            _logger.LogWarning("Received null DeviceMessage");         
        }
        else
        {
            _logger.LogInformation("Received DeviceMessage: {message}", input.ToString());
        }
        return ValueTask.CompletedTask;
    }
}
