namespace AutomationHUB.Messaging.Nats
{
    public interface INatsDomainTopicBuilder
    {
        string GetDomainTopic(Type publishable);
    }
}