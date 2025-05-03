using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Devices
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(MessageType))]
    [JsonDerivedType(typeof(DeviceMessage), nameof(DeviceMessage))]
    public class AutomationMessage
    {
        [JsonPropertyOrder(int.MinValue)]
        public string MessageType { get => GetType().Name; }
        public string AutomationID { get; protected set; } = default!;
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public override string ToString()
        {
            return $"Fields={string.Join(", ", Fields)}, Timestamp={Timestamp}";
        }
    }

    public class DeviceMessage : AutomationMessage
    {
        public string DeviceId { get => base.AutomationID; set => base.AutomationID = value; }
        public string DeviceType { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"DeviceMessage: DeviceId={DeviceId}, DeviceType={DeviceType}, {base.ToString()}";
        }
    }
}
