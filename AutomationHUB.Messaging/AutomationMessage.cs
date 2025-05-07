using AutomationHUB.Messaging.Attributes;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Registry;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutomationHUB.Messaging;

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(MessageType))]
[JsonDerivedType(typeof(DeviceMessage), nameof(DeviceMessage))]
[JsonDerivedType(typeof(DeviceRegistryMessage), nameof(DeviceRegistryMessage))]
public abstract class 
    AutomationMessage : INATSPublishable
{
    [JsonIgnore]
    public virtual string MessageType => GetType().Name;
    [NATSEntity]
    public abstract required string Entity { get; set; }
    [NATSEntityId]
    public abstract required string Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public byte[] ToNATSPayload()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this);
    }

    public override string ToString()
    {
        return $"MessageType={MessageType} [Timestamp={Timestamp}] Entity={Entity} Id={Id}";
    }
}
