using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Api.Contracts
{
    public static class SignalRRoutes
    {
        public const string DeviceHubPath = "/deviceHUB";
        public const string ReceiveDeviceMessage = nameof(ReceiveDeviceMessage);
    }
}
