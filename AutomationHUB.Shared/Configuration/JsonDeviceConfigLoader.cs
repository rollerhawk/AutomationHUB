using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutomationHUB.Shared.Configuration;

public class JsonDeviceConfigLoader
{
    public async Task<DeviceConfiguration> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Device config not found", filePath);

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return JsonSerializer.Deserialize<DeviceConfiguration>(json, options)
               ?? throw new InvalidOperationException("Failed to deserialize config.");
    }
}



