namespace AutomationHUB.Shared.Configuration.DataProcessor;

public class StxEtxProcessorConfig : ByteDataProcessorConfig
{
    public byte StartMarker { get; set; }    // z.B. 0x02
    public byte EndMarker { get; set; }    // z.B. 0x03
    public string SplitDelimiter { get; set; } = default!; // z.B. "\r\n"
}



