namespace AutomationHUB.Messaging.Registry
{
    public class DeviceRegistryMessage() : RegistryMessage
    {
        public override required string Entity { get; set; }
        public override required string Id { get; set; }
    }
}
