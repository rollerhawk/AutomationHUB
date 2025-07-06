using AutomationHUB.Shared.Configuration;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutomationHUB.Engine.Api.Contracts;

namespace AutomationHUB.Engine.Api.Services;

public class DeviceConfigurationService : IDeviceConfigurationService
{
    private readonly List<DeviceConfiguration> _deviceConfigs;

    public DeviceConfigurationService(IHostEnvironment env)
    {
        _deviceConfigs = new List<DeviceConfiguration>();

        // Pfad zum Config-Ordner (unter ContentRoot)
        var configDir = Path.Combine(env.ContentRootPath, "DeviceConfigs");
        if (!Directory.Exists(configDir))
            return;

        // JSON-Optionen für Polymorphie + Enums
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            // damit [JsonStringEnumConverter] greift
            Converters = { new JsonStringEnumConverter() }
        };

        foreach (var file in Directory.GetFiles(configDir, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                // Prüfen, ob Array oder Single-Objekt
                var trimmed = json.TrimStart();
                if (trimmed.StartsWith("["))
                {
                    var list = JsonSerializer.Deserialize<List<DeviceConfiguration>>(json, options);
                    if (list != null)
                        _deviceConfigs.AddRange(list);
                }
                else
                {
                    var single = JsonSerializer.Deserialize<DeviceConfiguration>(json, options);
                    if (single != null)
                        _deviceConfigs.Add(single);
                }
            }
            catch (Exception ex)
            {
                // TODO: Logging einbauen
                Console.Error.WriteLine($"Fehler beim Einlesen von {file}: {ex.Message}");
            }
        }
    }

    public Task<List<DeviceConfiguration>> GetAllAsync()
    {
        // Rückgabe einer Kopie, damit der interne Cache unverändert bleibt
        return Task.FromResult(_deviceConfigs.ToList());
    }


    public Task<DeviceConfiguration?> GetByIdAsync(string deviceId)
    {
        return Task.FromResult(_deviceConfigs.FirstOrDefault(x => x.DeviceId == deviceId));
    }
}
