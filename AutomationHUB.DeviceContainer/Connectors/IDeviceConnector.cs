using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.DeviceContainer.Connectors
{
    public interface IDeviceConnector
    {
        /// <summary>
        /// Initializes the device connection.
        /// </summary>
        /// <returns>True if the connection was successful, false otherwise.</returns>
        Task<bool> ConnectAsync(CancellationToken ct = default);
        /// <summary>
        /// Reads data from the device.
        /// </summary>
        /// <returns>The data read from the device.</returns>
        Task<byte[]> ReadAsync(CancellationToken ct = default);
        /// <summary>
        /// Closes the device connection.
        /// </summary>
        Task DisconnectAsync();
    }
}
