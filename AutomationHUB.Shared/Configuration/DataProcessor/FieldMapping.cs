using System.Text.Json.Serialization;
using AutomationHUB.Shared.Enum;

namespace AutomationHUB.Shared.Configuration.DataProcessor;

public class FieldMapping
{
    /// <summary>
    /// Zero-Based Index of the field in the raw data
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// Name of the field
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Type of the field
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FieldType Type { get; set; }
}

