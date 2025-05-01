using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Shared.Configuration;

namespace AutomationHUB.DeviceContainer
{
    public class Worker(
    ILogger<Worker> logger,
    IConfiguration config,
    IDeviceConnectorFactory connectorFactory,
    IByteDataProcessorFactory processorFactory,
    JsonDeviceConfigLoader loader) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IConfiguration _configuration = config;
        private readonly IDeviceConnectorFactory _connectorFactory = connectorFactory;
        private readonly IByteDataProcessorFactory _processorFactory = processorFactory;
        private readonly JsonDeviceConfigLoader _loader = loader;

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Device running at: {time}", DateTimeOffset.Now);
            try
            {
                if (_configuration["DeviceConfigPath"] is not string cfgPath)
                {
                    _logger.LogError("Device config path not set in configuration.");
                    return;
                }                

                var cfg = await _loader.LoadAsync(cfgPath, ct);
                _logger.LogInformation("Loaded device config: {config}", cfg);

                var processor = _processorFactory.Create(cfg.ProcessorConfig);                

                var connector = _connectorFactory.Create(cfg);

                if (await connector.ConnectAsync(ct))
                {
                    _logger.LogInformation("Connected to device.");
                }
                else
                {
                    _logger.LogError("Failed to connect to device.");
                    return;
                }

                while (!ct.IsCancellationRequested)
                {
                    var raw = await connector.ReadAsync(ct);         
                    
                    _logger.LogInformation("Data read: {data}", raw);                   

                    var processedFields = await processor.ProcessAsync(raw);

                    _logger.LogInformation("Processed fields: {fields}", string.Join(", ", processedFields));
                }

                _logger.LogInformation("Stopping device.");
                await connector.DisconnectAsync();
                _logger.LogInformation("Device stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred.");
            }
        }
    }
}
