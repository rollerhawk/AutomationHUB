using AutomationHUB.Messaging.Devices;
using Elsa.Expressions.Models;
using Elsa.Workflows;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Engine.Activities.Events;

/// <summary>
/// Elsa Activity for Handling DeviceMessage
/// </summary>
/// <param name="logger"></param>
public class DeviceMessageEvent() : AutomationMessageEvent<DeviceMessage>()
{
    public required string DeviceId { get => AutomationId; set => AutomationId = value; }
    protected override void OnEventReceived(ActivityExecutionContext context, DeviceMessage? input)
    {
        int a = 2;
    }
}
