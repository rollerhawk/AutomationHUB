using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Messaging;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace AutomationHUB.Portal.Services
{
    public class DeviceStateClient(HubConnection hub) : IDeviceStateClient
    {
        private readonly HubConnection _hub = hub;

        public event Action<DeviceMessage>? OnMessage;

        public async Task StartAsync()
        {
            // subscribe to whatever methodName you passed in
            _hub.On<DeviceMessage>(SignalRRoutes.ReceiveDeviceMessage, msg => OnMessage?.Invoke(msg));
            await _hub.StartAsync();
        }
    }
}
