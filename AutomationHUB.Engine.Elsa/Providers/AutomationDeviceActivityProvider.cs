using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutomationHUB.Engine.Elsa.Activities;
using Elsa.Models;
using Elsa.Workflows;
using Elsa.Workflows.Management.Options;
using Elsa.Workflows.Management.Providers;
using Elsa.Workflows.Models;
using Microsoft.Extensions.Options;

namespace AutomationHUB.Engine.Elsa.Providers;

public class AutomationDeviceActivityProvider : TypedActivityProvider, IActivityProvider
{
    private readonly IActivityDescriber _activityDescriber;
    public AutomationDeviceActivityProvider(IOptions<ManagementOptions> options, IActivityDescriber activityDescriber) : base(options, activityDescriber)
    {
        _activityDescriber = activityDescriber;
    }

    public new async ValueTask<IEnumerable<ActivityDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default)
    {
        var activityDescriptors = await base.GetDescriptorsAsync(cancellationToken);
        var list = new List<ActivityDescriptor>(activityDescriptors);        
        return list;
    }
}
