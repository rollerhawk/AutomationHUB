using AutomationHUB.Shared.Configuration.DataProcessor;
using AutomationHUB.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.DeviceContainer.Processors;

public class StxEtxProcessor(StxEtxProcessorConfig cfg, ILogger<StxEtxProcessor> logger) : IByteDataProcessor
{
    private readonly StxEtxProcessorConfig _cfg = cfg;
    private readonly ILogger<StxEtxProcessor> _logger = logger;

    public Dictionary<string, string> SegmentFields(byte[] rawData)
    {
        _logger.LogTrace("Segmenting raw data: {data}", BitConverter.ToString(rawData));
        // 1) Strip STX/ETX
        var withoutMarkers = rawData
            .SkipWhile(b => b != _cfg.StartMarker)
            .Skip(1)    // den Startmarker selbst überspringen
            .TakeWhile(b => b != _cfg.EndMarker)
            .ToArray();

        // 2) Split am Delimiter
        var text = Encoding.UTF8.GetString(withoutMarkers);
        var parts = text.Split(_cfg.SplitDelimiter, StringSplitOptions.None);
        _logger.LogTrace("Split data into {count} parts: {parts}", parts.Length, string.Join(',', parts));

        // 3) Map Strings zu Feld-Keys
        var result = new Dictionary<string, string>();

        foreach (var map in _cfg.FieldMappings)
        {
            _logger.LogTrace("Mapping {name} to index {index}", map.Name, map.Index);
            if (map.Index < 0 || map.Index >= parts.Length)
                continue;

            var str = parts[map.Index];
            _logger.LogTrace("Mapping {name} to value {value}", map.Name, str);
            result[map.Name] = str;
        }
        return result;
    }

    public Task<Dictionary<string, object>> ProcessAsync(byte[] rawData)
    {
        return Task.Run(() =>
        {
            var result = new Dictionary<string, object>();
            try
            {
                var segs = SegmentFields(rawData);

                foreach (var map in _cfg.FieldMappings)
                {
                    _logger.LogTrace("Processing {name} of type {type}", map.Name, map.Type);
                    var str = segs.TryGetValue(map.Name, out var s) ? s : string.Empty;
                    object val = map.Type switch
                    {
                        FieldType.DOUBLE => double.TryParse(str, out var d) ? d : 0.0,
                        FieldType.INT => int.TryParse(str, out var i) ? i : 0,
                        _ => str
                    };
                    _logger.LogTrace("Parsed {name} with {value} to {type}", map.Name, val, val.GetType());
                    result[map.Name] = val;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data: {message}", ex.Message);
            }
            return result;
        });
    }
}
