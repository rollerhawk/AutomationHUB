using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AutomationHUB.Engine.Api.SignalR
{
    public class DeviceMessageSignalRPublisher(IHubContext<DeviceHub> hubContext) : IMessageConsumer<DeviceMessage>
    {     
        private readonly IHubContext<DeviceHub> _hubContext = hubContext;

        public async Task HandleAsync(DeviceMessage message, CancellationToken ct)
        {
            await _hubContext.Clients.All.SendAsync(SignalRRoutes.ReceiveDeviceMessage, message, ct);
        }
    }
}
