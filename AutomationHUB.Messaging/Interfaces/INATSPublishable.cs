namespace AutomationHUB.Messaging.Interfaces
{
    public interface INATSPublishable
    {
        byte[] ToNATSPayload();
    }
}
