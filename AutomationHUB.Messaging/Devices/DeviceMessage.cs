using AutomationHUB.Messaging.Attributes;
using AutomationHUB.Messaging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Devices;

[NATSDomain(Domain = "devices")]
public class DeviceMessage : AutomationMessage, IHasFields
{    
    /// <summary>
    /// DeviceType
    /// </summary>
    public override required string Entity { get; set; }

    /// <summary>
    /// DeviceId
    /// </summary>
    public override required string Id { get; set; }

    public Dictionary<string, object> Fields { get; set; } = default!;

    public override string ToString()
    {
        return $"{base.ToString()} Fields=[{string.Join(',',Fields)}]";
    }
}
