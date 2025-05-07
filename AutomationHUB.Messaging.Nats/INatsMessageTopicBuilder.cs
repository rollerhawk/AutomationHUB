using AutomationHUB.Messaging.Interfaces;

namespace AutomationHUB.Messaging.Nats
{
    public interface INatsMessageTopicBuilder
    {
        string GetMessageTopic(INATSPublishable message);
    }
}