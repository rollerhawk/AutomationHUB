using AutomationHUB.Messaging.Devices;
using Elsa.Expressions.Models;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using Elsa.Workflows.UIHints.Dropdown;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Nodes;

namespace AutomationHUB.Engine.Elsa.Activities;

/// <summary>
/// Elsa Activity for Handling DeviceMessage
/// </summary>
/// <param name="logger"></param>
public class DeviceMessageEvent : AutomationMessageEvent<DeviceMessage>
{
    [Input]
    public string DeviceId { get => AutomationId; set => AutomationId = value; }

    protected override void OnEventReceived(ActivityExecutionContext context, DeviceMessage? input)
    {
        int a = 2;
    }
}

