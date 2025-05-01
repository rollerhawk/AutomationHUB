using AutomationHUB.DeviceContainer.Factories;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Shared.Configuration;
using AutomationHUB.Shared.Interfaces;
using NATS.Client;
using System.Text.Json;

namespace AutomationHUB.DeviceContainer
{
    public class Worker(
    ILogger<Worker> logger,
    IConfiguration config,
    IDeviceConnectorFactory connectorFactory,
    IByteDataProcessorFactory processorFactory,
    JsonDeviceConfigLoader loader,
    IPublisher<DeviceMessage> publisher) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IConfiguration _configuration = config;
        private readonly IDeviceConnectorFactory _connectorFactory = connectorFactory;
        private readonly IByteDataProcessorFactory _processorFactory = processorFactory;
        private readonly JsonDeviceConfigLoader _loader = loader;
        private readonly IPublisher<DeviceMessage> _publisher = publisher;
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

        private void PublishDeviceMessage(Dictionary<string, object> processedFields)
        {
            //device.device1
            var subject = $"{_deviceConfig.DeviceType}.{_deviceConfig.DeviceId}";

            var deviceMsg = new DeviceMessage
            {
                DeviceId = _deviceConfig.DeviceId,
                DeviceType = _deviceConfig.DeviceType,
                Fields = processedFields,
                Timestamp = DateTime.UtcNow
            };

            _publisher.Publish(subject, deviceMsg);

            _logger.LogInformation("Published data to subject: {subject}", subject);
        }
    }
}
