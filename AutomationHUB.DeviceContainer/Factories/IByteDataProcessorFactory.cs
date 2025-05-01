using AutomationHUB.DeviceContainer.Processors;
using AutomationHUB.Shared.Configuration.DataProcessor;

namespace AutomationHUB.DeviceContainer.Factories
{
    public interface IByteDataProcessorFactory
    {
        IByteDataProcessor Create(ByteDataProcessorConfig cfg);
    }
}
