using AutomationHUB.Shared.Configuration;

namespace AutomationHUB.Engine.Api.Contracts
{
    public interface IDeviceConfigurationService
    {
        Task<List<DeviceConfiguration>> GetAllAsync();

        Task<DeviceConfiguration?> GetByIdAsync(string deviceId);
    }
}
