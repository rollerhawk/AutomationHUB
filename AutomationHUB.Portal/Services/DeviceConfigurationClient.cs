using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Portal.HttpMessageHandlers;
using AutomationHUB.Shared.Configuration;

namespace AutomationHUB.Portal.Services
{
    public class DeviceConfigurationClient(HttpClient httpClient) : IDeviceConfigurationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<DeviceConfiguration>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ApiRoutes.DeviceConfigurations);

            return (await _httpClient.GetFromJsonAsync<List<DeviceConfiguration>>(ApiRoutes.DeviceConfigurations))!;
        }
        public async Task<DeviceConfiguration?> GetByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<DeviceConfiguration?>(ApiRoutes.DeviceConfigurationById.Replace("{id}", id));
        }       
    }
}
