using AutomationHUB.DeviceContainer.Processors;
using AutomationHUB.Shared.Configuration.DataProcessor;

namespace AutomationHUB.DeviceContainer.Factories
{
    public class ByteDataProcessorFactory(IServiceProvider sp) : IByteDataProcessorFactory
    {
        private readonly IServiceProvider _sp = sp;
        public IByteDataProcessor Create(ByteDataProcessorConfig cfg)
        {
            return cfg switch
            {
                StxEtxProcessorConfig stxEtxCfg => ActivatorUtilities.CreateInstance<StxEtxProcessor>(_sp, stxEtxCfg),
                _ => throw new NotSupportedException($"Data processor type {cfg.GetType()} is not supported.")
            };
        }
    }
}
