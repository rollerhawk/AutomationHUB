using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Portal.ViewModels;
using AutomationHUB.Shared.Configuration;
using MudBlazor;

namespace AutomationHUB.Portal.Services;

public class PortalDeviceService
{
    private readonly IDeviceConfigurationService _cfgClient;
    private readonly IDeviceStateService _stateClient;

    private readonly Dictionary<string, DeviceConfiguration> _configs
        = new();
    private readonly Dictionary<string, DeviceMessage> _messages
        = new();

    public event Action? OnChange;

    public PortalDeviceService(
        IDeviceConfigurationService cfgClient,
        IDeviceStateService stateClient)
    {
        _cfgClient = cfgClient;
        _stateClient = stateClient;
        _stateClient.OnDeviceChanged += HandleDeviceMessage;
    }

    public async Task InitializeAsync()
    {
        var list = await _cfgClient.GetAllAsync();
        foreach (var c in list)
            _configs[c.DeviceId] = c;

        OnChange?.Invoke();
    }

    private void HandleDeviceMessage(string deviceId, DeviceMessage msg)
    {
        _messages[deviceId] = msg;
        OnChange?.Invoke();
    }

    /// <summary>Gibt für jedes konfigurierte Gerät das ViewModel zurück.</summary>
    public IEnumerable<DeviceViewModel> GetDevices()
    {
        foreach (var cfg in _configs.Values)
        {
            var vm = new DeviceViewModel(cfg.DeviceId, cfg.DeviceType, Icons.Material.Filled.QrCodeScanner, true);

            // statische Meta
            vm.StaticMeta["Protocol"] = cfg.Connection.Protocol;
            vm.StaticMeta["Address"] = cfg.Connection.Address;
            // hier weitere Felder aus cfg.ProcessorConfig…

            // dynamische Meta
            if (_messages.TryGetValue(cfg.DeviceId, out var m))
            {
                foreach (var kv in m.Fields)
                    vm.DynamicMeta[kv.Key] = kv.Value;
            }

            yield return vm;
        }
    }
}
