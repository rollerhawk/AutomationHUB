using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutomationHUB.Shared.Configuration;

public class JsonDeviceConfigLoader(IConfiguration configuration, ILogger<IDeviceConfigLoader> logger) : IDeviceConfigLoader
{
    private readonly static JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
    };

    public DeviceConfiguration? GetConfig()
    {
        if (configuration["DeviceConfigPath"] is not string cfgPath)
        {
            logger.LogError("Device config path not set in configuration.");
            return null;
        }
        return LoadAsync(cfgPath).Result;
    }

    private async Task<DeviceConfiguration> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Device config not found", filePath);

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize<DeviceConfiguration>(json, _options)
               ?? throw new InvalidOperationException("Failed to deserialize config.");
    }
}



