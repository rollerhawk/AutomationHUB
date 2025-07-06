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
    IDeviceConfigLoader deviceConfigLoader,
    IDeviceConnectorFactory connectorFactory,
    IByteDataProcessorFactory processorFactory,
    IPublisher publisher) : BackgroundService
    {
        private readonly ILogger<DeviceService> _logger = logger;
        private readonly IDeviceConnectorFactory _connectorFactory = connectorFactory;
        private readonly IByteDataProcessorFactory _processorFactory = processorFactory;
        private readonly IPublisher _publisher = publisher;
        private readonly DeviceConfiguration _deviceConfig = deviceConfigLoader.GetConfig() ?? throw new InvalidOperationException("Device configuration is null. Ensure the config loader is properly set up.");

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Device running at: {time}", DateTimeOffset.Now);
            try
            {
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
