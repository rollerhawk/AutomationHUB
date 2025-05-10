using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;

namespace AutomationHUB.Portal.Services
{
    public class DeviceStateService : IMessageConsumer<DeviceMessage>, IDeviceStateService
    {
        public event Action<string, DeviceMessage>? OnDeviceChanged;

        public Task HandleAsync(DeviceMessage message, CancellationToken ct)
        {
            OnDeviceChanged?.Invoke(message.Id, message);
            return Task.CompletedTask;
        }
    }
}
