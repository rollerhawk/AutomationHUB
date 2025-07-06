using AutomationHUB.DeviceContainer.Connectors;
using AutomationHUB.Shared.Configuration;
using AutomationHUB.Shared.Enum;

namespace AutomationHUB.DeviceContainer.Factories
{
    public class DeviceConnectorFactory(IServiceProvider sp) : IDeviceConnectorFactory
    {
        private readonly IServiceProvider _sp = sp;

        public IDeviceConnector Create(DeviceConfiguration cfg)
        {
            return cfg.Connection.Protocol switch
            {
                ProtocolType.TCP => ActivatorUtilities.CreateInstance<TcpDeviceConnector>(_sp, cfg.Connection.Address),                
                _ => throw new NotSupportedException($"Protocol {cfg.Connection.Protocol} is not supported")
            };
        }
    }
}
