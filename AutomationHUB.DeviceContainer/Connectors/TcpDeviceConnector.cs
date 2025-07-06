using AutomationHUB.Shared.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace AutomationHUB.DeviceContainer.Connectors
{
    public class TcpDeviceConnector : IDeviceConnector
    {
        private readonly ILogger _logger;
        private readonly TcpConnectionInfo _config;
        private readonly string _host;
        private readonly int _port;

        private TcpClient? _client;
        private NetworkStream? _stream;

        public TcpDeviceConnector(TcpConnectionInfo config, ILogger<TcpDeviceConnector> logger)
        {
            _config = config;
            var address = config.Address;

            var parts = address.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !int.TryParse(parts[1], out _port))
                throw new ArgumentException($"Invalid TCP address: '{address}'. Expected 'host:port'.", nameof(address));

            _host = parts[0];
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ConnectAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting TCP connect to {Host}:{Port}", _host, _port);
                    _client = new TcpClient();
                    await _client.ConnectAsync(_host, _port, ct);
                    _stream = _client.GetStream();
                    _logger.LogInformation("TCP connected to {Host}:{Port}", _host, _port);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "TCP connect failed to {Host}:{Port}. Retrying in {_config.ReconnectDelaySeconds} seconds.", _host, _port, _config.ReconnectDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(_config.ReconnectDelaySeconds), ct);
                }
            }

            return false;
        }

        public async Task<byte[]> ReadAsync(CancellationToken ct = default)
        {
            if (_stream is null)
                throw new InvalidOperationException("Not connected");

            using var ms = new MemoryStream();
            var buffer = new byte[4096];

            while (!_stream.DataAvailable)
            {
                _logger.LogDebug("Waiting for data on TCP {Host}:{Port}", _host, _port);
                await Task.Delay(10, ct);
                if (ct.IsCancellationRequested) break;
            }

            int read;
            while ((read = await _stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                ms.Write(buffer, 0, read);
                if (!_stream.DataAvailable) break;
            }

            var data = ms.ToArray();
            _logger.LogTrace("TCP read {ByteCount} bytes: {bytes}", data.Length, BitConverter.ToString(data));
            return data;
        }

        public Task DisconnectAsync()
        {
            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
                _logger.LogInformation("TCP disconnected from {Host}:{Port}", _host, _port);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while disconnecting TCP {Host}:{Port}", _host, _port);
            }
            return Task.CompletedTask;
        }
    }

}
