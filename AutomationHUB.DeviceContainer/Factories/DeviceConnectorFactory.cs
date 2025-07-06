using AutomationHUB.DeviceContainer.Connectors;
using AutomationHUB.Shared.Configuration;
using AutomationHUB.Shared.Enum;

namespace AutomationHUB.DeviceContainer.Factories
{
    public class DeviceConnectorFactory(IServiceProvider sp, IConfiguration configuration) : IDeviceConnectorFactory
    {
        private readonly IServiceProvider _sp = sp;

        public IDeviceConnector Create(DeviceConfiguration cfg)
        {
            return cfg.Connection.Protocol switch
            {
                ProtocolType.TCP => CreateTcpDeviceConnector((TcpConnectionInfo)cfg.Connection),
                _ => throw new NotSupportedException($"Protocol {cfg.Connection.Protocol} is not supported")
            };
        }

        private TcpDeviceConnector CreateTcpDeviceConnector(TcpConnectionInfo cfg)
        {
            return ActivatorUtilities.CreateInstance<TcpDeviceConnector>(_sp, cfg);
        }
    }
}
