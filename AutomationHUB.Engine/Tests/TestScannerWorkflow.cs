using AutomationHUB.Engine.Activities.Events;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Runtime.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Tests;

public class TestScannerWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
            {
                new DeviceMessageEvent
                {                    
                    CanStartWorkflow = true,
                    DeviceId = "scanner1"
                },
                new WriteLine("Workflow Started"),
                new DeviceMessageEvent
                {
                    DeviceId = "scanner2"
                },
                new WriteLine("Event Received")
            }
        };
    }
}