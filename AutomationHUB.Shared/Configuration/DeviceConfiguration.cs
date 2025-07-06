using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutomationHUB.Shared.Configuration.DataProcessor;
using AutomationHUB.Shared.Enum;

namespace AutomationHUB.Shared.Configuration;

public class DeviceConfiguration
{
    public string DeviceId { get; set; } = default!;
    public string DeviceType { get; set; } = default!;
    public ConnectionInfo Connection { get; set; } = default!;
    public ByteDataProcessorConfig ProcessorConfig { get; set; } = default!;
}

public class ConnectionInfo
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProtocolType Protocol { get; set; }
    public string Address { get; set; } = default!;
}
