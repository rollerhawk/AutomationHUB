using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Registry;
using AutomationHUB.Shared.Configuration;
using NATS.Client;
using System.Text.Json;

namespace AutomationHUB.DeviceContainer
{
    public class DeviceService(
    ILogger<DeviceService> logger,
    IConfiguration config,
    IDeviceConnectorFactory connectorFactory,
    IByteDataProcessorFactory processorFactory,
    JsonDeviceConfigLoader loader,
    IPublisher publisher) : BackgroundService
    {
        private readonly ILogger<DeviceService> _logger = logger;
        private readonly IConfiguration _configuration = config;
        private readonly IDeviceConnectorFactory _connectorFactory = connectorFactory;
        private readonly IByteDataProcessorFactory _processorFactory = processorFactory;
        private readonly JsonDeviceConfigLoader _loader = loader;
        private readonly IPublisher _publisher = publisher;
        private DeviceConfiguration _deviceConfig = null!;

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

                _deviceConfig = await _loader.LoadAsync(cfgPath, ct);

                _logger.LogInformation("Loaded device config: {config}", _deviceConfig);

                var processor = _processorFactory.Create(_deviceConfig.ProcessorConfig);

                var connector = _connectorFactory.Create(_deviceConfig);

                if (await connector.ConnectAsync(ct))
                {
                    _logger.LogInformation("Connected to device.");

                    PublishRegistryMessage();
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

                    PublishDeviceMessage(processedFields);
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

        private void PublishRegistryMessage()
        {
            var regMsg = new DeviceRegistryMessage
            {
                Id = _deviceConfig.DeviceId,
                Entity = _deviceConfig.DeviceType
            };

            var topic = _publisher.Publish(regMsg);

            _logger.LogInformation("Published device registry message: {message}", JsonSerializer.Serialize(regMsg));

            _logger.LogInformation("Published data to topic: {subject}", topic);
        }

        private void PublishDeviceMessage(Dictionary<string, object> processedFields)
        {
            var deviceMsg = new DeviceMessage
            {
                Id = _deviceConfig.DeviceId,
                Entity = _deviceConfig.DeviceType,
                Fields = processedFields
            };

            var topic = _publisher.Publish(deviceMsg);

            _logger.LogInformation("Published data to topic: {subject}", topic);
        }
    }
}
