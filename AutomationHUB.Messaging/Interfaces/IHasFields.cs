namespace AutomationHUB.Messaging.Interfaces;

public interface IHasFields
{
    Dictionary<string, object> Fields { get; set; }
}
