using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Devices
{
    public class DeviceMessage
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"DeviceMessage: DeviceId={DeviceId}, DeviceType={DeviceType}, Fields={string.Join(", ", Fields)}, Timestamp={Timestamp}";
        }
    }
}
