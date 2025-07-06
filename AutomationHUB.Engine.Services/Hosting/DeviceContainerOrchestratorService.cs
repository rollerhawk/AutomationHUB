using AutomationHUB.Engine.Api.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Engine.Services.Hosting;

public class DeviceContainerOrchestratorService(IDeviceContainerHostService deviceContainerHostService, IDeviceConfigurationService deviceConfigurationService, ILogger<DeviceContainerOrchestratorService> logger) : BackgroundService
{
    private readonly IDeviceContainerHostService _deviceContainerHostService = deviceContainerHostService;
    private readonly IDeviceConfigurationService _deviceConfigurationService = deviceConfigurationService;
    private readonly ILogger<DeviceContainerOrchestratorService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var deviceConfig in await _deviceConfigurationService.GetAllAsync())
        {
            _logger.LogInformation("Starting device container for DeviceId: {DeviceId}", deviceConfig.DeviceId);
            _deviceContainerHostService.StartDeviceContainer(deviceConfig);
            _logger.LogInformation("Started device container for DeviceId: {DeviceId}", deviceConfig.DeviceId);
        }
    }
}
