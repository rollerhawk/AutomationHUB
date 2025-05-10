using AutomationHUB.Messaging.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Api.Contracts
{
    public interface IDeviceStateService
    {
        /// <summary>Wird gefeuert, wenn für ein Gerät eine neue Nachricht ankommt.</summary>
        event Action<string, DeviceMessage>? OnDeviceChanged;
    }
}
