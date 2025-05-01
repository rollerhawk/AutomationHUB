using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.DeviceContainer.Processors
{
    public interface IByteDataProcessor
    {
        /// <summary>
        /// Nimmt rohe Bytes und liefert ein Dictionary der rohen Segment-Strings.
        /// </summary>
        Dictionary<string, string> SegmentFields(byte[] rawData);

        /// <summary>
        /// Führt anschließend Typ-Casting gemäß FieldMappings durch
        /// </summary>
        Task<Dictionary<string, object>> ProcessAsync(byte[] rawData);
    }
}
