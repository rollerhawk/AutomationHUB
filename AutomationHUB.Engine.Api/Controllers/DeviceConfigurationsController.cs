using AutomationHUB.Engine.Api.Services;
using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Shared.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace AutomationHUB.Engine.Api.Controllers;

[ApiController]
[Route(ApiRoutes.DeviceConfigurations)]
public class DeviceConfigurationsController(IDeviceConfigurationService configService) : ControllerBase
{
    private readonly IDeviceConfigurationService _configService = configService;

    /// <summary>
    /// Get all configured devices
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DeviceConfiguration>>> GetAll()
    {
        var configs = await _configService.GetAllAsync();
        return Ok(configs);
    }

    [HttpGet(ApiRoutes.DeviceConfigurationById)]
    public async Task<ActionResult<DeviceConfiguration>> GetById(string id)
    {
        var cfg = await _configService.GetByIdAsync(id);
        return cfg is null ? NotFound() : Ok(cfg);
    }
}
