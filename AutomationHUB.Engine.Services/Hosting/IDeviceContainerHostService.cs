using AutomationHUB.Shared.Configuration;

namespace AutomationHUB.Engine.Services.Hosting;

public interface IDeviceContainerHostService
{
    void StartDeviceContainer(DeviceConfiguration config);
    Task<bool> StartDeviceContainerByDeviceIdAsync(string deviceId);
    Task StopDeviceContainerAsync(string deviceId);
}
