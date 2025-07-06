namespace AutomationHUB.Shared.Configuration;

public class InstanceDeviceConfigLoader(DeviceConfiguration instance) : IDeviceConfigLoader
{
    public DeviceConfiguration? GetConfig()
    {
        return instance;
    }
}



