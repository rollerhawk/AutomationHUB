namespace AutomationHUB.Portal.ViewModels;

public class DeviceViewModel(string deviceId, string deviceType, string icon, bool isConnected, DateTime? lastUpdate = null)
{
    public string DisplayName => DeviceId;

    public string Icon { get; set; } = icon;

    /// <summary>
    /// Eindeutige Geräte-ID, z. B. „Scanner01“.
    /// </summary>
    public string DeviceId { get; set; } = deviceId;

    /// <summary>
    /// Typ des Geräts, z. B. „Scanner“ oder „Waage“.
    /// </summary>
    public string DeviceType { get; set; } = deviceType;

    public bool IsConnected { get; set; } = isConnected;

    /// <summary>
    /// Statische Metadaten aus der Konfiguration (DeviceConfiguration).
    /// </summary>
    public Dictionary<string, object?> StaticMeta { get; } = new();

    /// <summary>
    /// Dynamische Metadaten aus der letzten DeviceMessage.
    /// </summary>
    public Dictionary<string, object?> DynamicMeta { get; set; } = new();

    /// <summary>
    /// Zeitstempel der letzten Nachricht (falls gewünscht).
    /// </summary>
    public DateTime? LastUpdate { get; set; } = lastUpdate;

    /// <summary>
    /// Kombinierte Auflistung aller Metadaten (für einfache Bindung).
    /// </summary>
    public IEnumerable<KeyValuePair<string, object?>> Metadata
    {
        get
        {
            foreach (var kv in StaticMeta)
                yield return kv;
            foreach (var kv in DynamicMeta)
                yield return kv;
        }
    }
}
