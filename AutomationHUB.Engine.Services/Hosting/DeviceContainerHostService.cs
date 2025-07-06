using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.DeviceContainer.Bootstrapper;
using Microsoft.Extensions.Options;
using AutomationHUB.Messaging.Nats.Configuration;
using Microsoft.Extensions.Hosting;
using AutomationHUB.Shared.Configuration;

namespace AutomationHUB.Engine.Services.Hosting;

/// <summary>
/// Hostet und verwaltet Devices Container
/// </summary>
public class DeviceContainerHostService(IDeviceConfigurationService deviceConfigurationService, IOptions<NatsOptions> natsOptions) : IDeviceContainerHostService
{
    private readonly NatsOptions _natsOptions = natsOptions.Value;
    private readonly IDeviceConfigurationService _deviceConfigurationService = deviceConfigurationService;
    private readonly Dictionary<string, IHost> _runningContainers = new();

    public async Task<bool> StartDeviceContainerByDeviceIdAsync(string deviceId)
    {
        if (_runningContainers.ContainsKey(deviceId))
            return true;

        var config = await _deviceConfigurationService.GetByIdAsync(deviceId);
        if (config == null)
            return false;
        StartDeviceContainer(config);
        return true;
    }

    public void StartDeviceContainer(DeviceConfiguration config)
    {
        var host = DeviceContainerBootstrapper.BuildAndStartHost(config, _natsOptions);
        _runningContainers[config.DeviceId] = host;
    }

    public Task StopDeviceContainerAsync(string deviceId)
    {
        if (_runningContainers.TryGetValue(deviceId, out var host))
        {
            _runningContainers.Remove(deviceId);
            return host.StopAsync();
        }
        return Task.CompletedTask;
    }
}
