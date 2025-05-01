using AutomationHUB.DeviceContainer.Connectors;
using AutomationHUB.Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.DeviceContainer.Factories
{
    public interface IDeviceConnectorFactory
    {
        IDeviceConnector Create(DeviceConfiguration cfg);
    }
}
